using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool
{
    public static class IconCreator
    {
        // Fields
        private static readonly int _iconWidth = 400;

        // Methods
        private static Size CalculateSize(Size clientSize, double filmRadio)
        {
            Size size = new Size();
            double num = ((double)clientSize.Height) / ((double)clientSize.Width);
            if (num > filmRadio)
            {
                size.Height = clientSize.Height;
                size.Width = (int)(((double)clientSize.Height) / filmRadio);
                return size;
            }
            size.Width = clientSize.Width;
            size.Height = (int)(clientSize.Width * filmRadio);
            return size;
        }

        public static Bitmap CreatePresentationImageIcon(IPresentationImage image, double tileRatio)
        {
            ISpatialTransformProvider provider = image as ISpatialTransformProvider;
            if (provider == null)
            {
                throw new Exception(SR.ConvertTransformProviderFailed);
            }
            ImageSpatialTransform spatialTransform = provider.SpatialTransform as ImageSpatialTransform;
            object memento = spatialTransform.CreateMemento();
            Size size = CalculateSize(image.ClientRectangle.Size, tileRatio);
            Bitmap bitmap = new Bitmap(image.DrawToBitmap(size.Width, size.Height), _iconWidth, Convert.ToInt32((float)(_iconWidth * Convert.ToSingle(tileRatio))));
            spatialTransform.SetMemento(memento);
            return bitmap;
        }

        public static Bitmap CreatePresentationImageIcon(IPresentationImage image, RectangleF destRange, double tileRatio)
        {
            if (destRange == RectangleF.Empty)
            {
                return null;
            }
            ISpatialTransformProvider provider = image as ISpatialTransformProvider;
            if (provider == null)
            {
                throw new Exception(SR.ConvertTransformProviderFailed);
            }
            ImageSpatialTransform spatialTransform = provider.SpatialTransform as ImageSpatialTransform;
            object memento = spatialTransform.CreateMemento();
            spatialTransform.MoveTo(destRange);
            Size destSize = new Size(Convert.ToInt32(destRange.Width), Convert.ToInt32(destRange.Height));
            Bitmap original = DrawToFilmSizeBitmap(destSize, image, tileRatio, true);
            Bitmap bitmap2 = new Bitmap(original, _iconWidth, (int)(_iconWidth * tileRatio));
            original.Dispose();
            spatialTransform.SetMemento(memento);
            return bitmap2;
        }

        public static Bitmap CreatePresentationImagePrintData(IPresentationImage image, double tileRatio, bool withAnnotation)
        {
            Size size;
            ISpatialTransformProvider provider = image as ISpatialTransformProvider;
            if (provider == null)
            {
                throw new Exception("转换失败");
            }
            ImageSpatialTransform spatialTransform = provider.SpatialTransform as ImageSpatialTransform;
            object memento = spatialTransform.CreateMemento();
            if (!spatialTransform.ScaleToFit)
            {
                float num = 1f / spatialTransform.Scale;
                int width = Convert.ToInt32((float)(image.ClientRectangle.Width * num));
                int height = Convert.ToInt32((float)(image.ClientRectangle.Height * num));
                if ((width > spatialTransform.SourceWidth) || (height > spatialTransform.SourceHeight))
                {
                    float num4 = ((float)image.ClientRectangle.Width) / ((float)image.ClientRectangle.Height);
                    float num5 = ((float)spatialTransform.SourceWidth) / ((float)spatialTransform.SourceHeight);
                    if (num4 > num5)
                    {
                        size = new Size(spatialTransform.SourceWidth, Convert.ToInt32((float)(((float)spatialTransform.SourceWidth) / num4)));
                        spatialTransform.Scale = (spatialTransform.Scale * spatialTransform.SourceWidth) / ((float)image.ClientRectangle.Width);
                    }
                    else
                    {
                        size = new Size(Convert.ToInt32((float)(spatialTransform.SourceHeight * num4)), spatialTransform.SourceHeight);
                        spatialTransform.Scale = (spatialTransform.Scale * spatialTransform.SourceHeight) / ((float)image.ClientRectangle.Height);
                    }
                }
                else
                {
                    size = new Size(width, height);
                    spatialTransform.Scale = (spatialTransform.Scale * width) / ((float)image.ClientRectangle.Width);
                }
            }
            else
            {
                size = new Size(spatialTransform.SourceWidth, spatialTransform.SourceHeight);
            }
            Size size2 = CalculateSize(size, tileRatio);
            if (!withAnnotation)
            {
                HideAnnotation(image);
            }
            Bitmap bitmap = image.DrawToBitmap(size2.Width, size2.Height);
            if (!withAnnotation)
            {
                ShowAnnotation(image);
            }
            spatialTransform.SetMemento(memento);
            return bitmap;
        }

        public static Bitmap CreatePresentationImagePrintData(IPresentationImage image, RectangleF destRange, double tileRatio, float scale, bool withAnnotation)
        {
            ISpatialTransformProvider provider = image as ISpatialTransformProvider;
            if (provider == null)
            {
                throw new Exception("转换失败");
            }
            ImageSpatialTransform spatialTransform = provider.SpatialTransform as ImageSpatialTransform;
            object memento = spatialTransform.CreateMemento();
            float num = scale / spatialTransform.Scale;
            spatialTransform.MoveTo(destRange, scale);
            Size destSize = new Size(Convert.ToInt32((float)(destRange.Width * num)), Convert.ToInt32((float)(destRange.Height * num)));
            Bitmap bitmap = DrawToFilmSizeBitmap(destSize, image, tileRatio, withAnnotation);
            spatialTransform.SetMemento(memento);
            return bitmap;
        }

        private static void DrawAnnotationBox(System.Drawing.Graphics g, Rectangle destination, string annotationText, AnnotationBox annotationBox)
        {
            Font font;
            Rectangle rect = RectangleUtilities.CalculateSubRectangle(destination, annotationBox.NormalizedRectangle);
            Rectangle.Inflate(rect, -4, -4);
            int num = (rect.Height / annotationBox.NumberOfLines) - 1;
            StringFormat stringFormat = new StringFormat();
            if (annotationBox.Truncation == AnnotationBox.TruncationBehaviour.Truncate)
            {
                stringFormat.Trimming = StringTrimming.Character;
            }
            else
            {
                stringFormat.Trimming = StringTrimming.EllipsisCharacter;
            }
            if (annotationBox.FitWidth)
            {
                stringFormat.Trimming = StringTrimming.None;
            }
            if (annotationBox.Justification == AnnotationBox.JustificationBehaviour.Right)
            {
                stringFormat.Alignment = StringAlignment.Far;
            }
            else if (annotationBox.Justification == AnnotationBox.JustificationBehaviour.Center)
            {
                stringFormat.Alignment = StringAlignment.Center;
            }
            else
            {
                stringFormat.Alignment = StringAlignment.Near;
            }
            if (annotationBox.VerticalAlignment == AnnotationBox.VerticalAlignmentBehaviour.Top)
            {
                stringFormat.LineAlignment = StringAlignment.Near;
            }
            else if (annotationBox.VerticalAlignment == AnnotationBox.VerticalAlignmentBehaviour.Center)
            {
                stringFormat.LineAlignment = StringAlignment.Center;
            }
            else
            {
                stringFormat.LineAlignment = StringAlignment.Far;
            }
            stringFormat.FormatFlags = StringFormatFlags.NoClip;
            if (annotationBox.NumberOfLines == 1)
            {
                stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
            }
            FontStyle regular = FontStyle.Regular;
            if (annotationBox.Bold)
            {
                regular |= FontStyle.Bold;
            }
            if (annotationBox.Italics)
            {
                regular |= FontStyle.Italic;
            }
            try
            {
                font = new Font(annotationBox.Font, (float)num, regular, GraphicsUnit.Pixel);
            }
            catch (Exception exception)
            {
                Platform.Log(LogLevel.Error, exception);
                font = new Font(AnnotationBox.DefaultFont, (float)num, FontStyle.Regular, GraphicsUnit.Pixel);
            }
            SizeF layoutArea = new SizeF((float)rect.Width, (float)rect.Height);
            SizeF ef2 = g.MeasureString(annotationText, font, layoutArea, stringFormat);
            if (annotationBox.FitWidth && (ef2.Width > rect.Width))
            {
                num = (int)Math.Round((double)((((double)(num * rect.Width)) / ((double)ef2.Width)) - 0.5));
                try
                {
                    font = new Font(annotationBox.Font, (float)num, regular, GraphicsUnit.Pixel);
                }
                catch (Exception exception2)
                {
                    Platform.Log(LogLevel.Error, exception2);
                    font = new Font(AnnotationBox.DefaultFont, (float)num, FontStyle.Regular, GraphicsUnit.Pixel);
                }
            }
            SolidBrush brush = new SolidBrush(Color.Black);
            rect.Offset(1, 1);
            g.DrawString(annotationText, font, brush, rect, stringFormat);
            brush.Color = Color.FromName(annotationBox.Color);
            rect.Offset(-1, -1);
            g.DrawString(annotationText, font, brush, rect, stringFormat);
            brush.Dispose();
            font.Dispose();
        }

        public static void DrawTextOverlay(System.Drawing.Graphics g, Rectangle destination, IPresentationImage presentationImage)
        {
            if ((presentationImage != null) && (presentationImage is IAnnotationLayoutProvider))
            {
                IAnnotationLayout annotationLayout = ((IAnnotationLayoutProvider)presentationImage).AnnotationLayout;
                if (annotationLayout != null)
                {
                    foreach (AnnotationBox box in annotationLayout.AnnotationBoxes)
                    {
                        if (box.Visible)
                        {
                            string annotationText = box.GetAnnotationText(presentationImage);
                            if (!string.IsNullOrEmpty(annotationText))
                            {
                                DrawAnnotationBox(g, destination, annotationText, box);
                            }
                        }
                    }
                }
            }
        }

        private static Bitmap DrawToFilmSizeBitmap(Size destSize, IPresentationImage image, double tileRatio, bool withAnnotation)
        {
            HideAnnotation(image);
            Bitmap bitmap = image.DrawToBitmap(destSize.Width, destSize.Height);
            Size size = CalculateSize(destSize, tileRatio);
            if ((size.Width & 1) == 1)
            {
                size.Width++;
            }
            Rectangle destination = new Rectangle(0, 0, size.Width, size.Height);
            Bitmap bitmap2 = new Bitmap(size.Width, size.Height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap2);
            if (size.Height > destSize.Height)
            {
                g.DrawImage(bitmap, 0, (size.Height - destSize.Height) / 2);
            }
            else
            {
                g.DrawImage(bitmap, (size.Width - destSize.Width) / 2, 0);
            }
            ShowAnnotation(image);
            if (withAnnotation)
            {
                DrawTextOverlay(g, destination, image);
            }
            g.Dispose();
            return bitmap2;
        }

        private static void HideAnnotation(IPresentationImage image)
        {
            if (image is IAnnotationLayoutProvider)
            {
                IAnnotationLayout annotationLayout = ((IAnnotationLayoutProvider)image).AnnotationLayout;
                if (annotationLayout != null)
                {
                    annotationLayout.Visible = false;
                }
            }
        }

        private static void ShowAnnotation(IPresentationImage image)
        {
            if (image is IAnnotationLayoutProvider)
            {
                IAnnotationLayout annotationLayout = ((IAnnotationLayoutProvider)image).AnnotationLayout;
                if (annotationLayout != null)
                {
                    annotationLayout.Visible = true;
                }
            }
        }
    }

}
