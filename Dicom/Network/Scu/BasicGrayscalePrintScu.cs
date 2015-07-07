using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System;
using System.Runtime.Remoting.Messaging;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Network.Scu
{
    public class BasicGrayscalePrintScu : ScuBase
    {
        // Fields
        private BasicFilmBoxModuleIod _basicFilmBoxModuleIod;
        private BasicFilmSessionModuleIod _basicFilmSessionModuleIod;
        private int _currentImageBoxIndex;
        private bool _disposed;
        private DicomUid _filmBoxUid;
        private string _filmSessionUid;
        private DicomAttributeCollection _firstFilmBoxResponseMessage;
        private IList<ImageBoxPixelModuleIod> _imageBoxPixelModuleIods;
        private string _imageBoxUid;
        private RequestType _nextRequestType;
        private DicomAttributeCollection _results;

        // Methods
        public IAsyncResult BeginPrint(string clientAETitle, string remoteAE, string remoteHost, int remotePort, BasicFilmSessionModuleIod basicFilmSessionModuleIod, BasicFilmBoxModuleIod basicFilmBoxModuleIod, IList<ImageBoxPixelModuleIod> imageBoxPixelModuleIods, AsyncCallback callback, object asyncState)
        {
            PrintDelegate delegate2 = new PrintDelegate(this.Print);
            return delegate2.BeginInvoke(clientAETitle, remoteAE, remoteHost, remotePort, basicFilmSessionModuleIod, basicFilmBoxModuleIod, imageBoxPixelModuleIods, callback, asyncState);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                base.Dispose(true);
                this._disposed = true;
            }
        }

        public DicomState EndPrint(IAsyncResult ar)
        {
            PrintDelegate asyncDelegate = ((AsyncResult)ar).AsyncDelegate as PrintDelegate;
            if (asyncDelegate == null)
            {
                throw new InvalidOperationException("cannot get results, asynchresult is null");
            }
            return asyncDelegate.EndInvoke(ar);
        }

        public override void OnReceiveAssociateAccept(DicomClient client, ClientAssociationParameters association)
        {
            base.OnReceiveAssociateAccept(client, association);
            if (base.Canceled)
            {
                client.SendAssociateAbort(DicomAbortSource.ServiceUser, DicomAbortReason.NotSpecified);
            }
            else
            {
                this.SendCreateFilmSessionRequest(client, association);
            }
        }

        public override void OnReceiveResponseMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, DicomMessage message)
        {
            try
            {
                base.ResultStatus = message.Status.Status;
                if (message.Status.Status == DicomState.Success)
                {
                    Platform.Log(LogLevel.Info, "Success status received in Printer Status Scu!");
                    this._results = message.DataSet;
                    switch (this._nextRequestType)
                    {
                        case RequestType.None:
                        case RequestType.FilmSession:
                            return;

                        case RequestType.FilmBox:
                            this.SendCreateFilmBoxRequest(client, association, message);
                            return;

                        case RequestType.ImageBox:
                            this.SendSetImageBoxRequest(client, association, message);
                            return;

                        case RequestType.PrintAction:
                            this.SendActionPrintRequest(client, association, message);
                            return;

                        case RequestType.DeleteFilmBox:
                            this.SendDeleteFilmBoxRequest(client, association, message);
                            return;

                        case RequestType.DeleteFilmSession:
                            this.SendDeleteFilmSessionRequest(client, association, message);
                            return;

                        case RequestType.Close:
                            base.ReleaseConnection(client);
                            return;
                    }
                }
                else
                {
                    Platform.Log(LogLevel.Info, string.Format("warning status: code:{0}, description: {1}", message.Status.Code, message.Status.Description));
                    base.ReleaseConnection(client);
                }
            }
            catch (Exception exception)
            {
                Platform.Log(LogLevel.Error, exception.ToString());
                base.ReleaseConnection(client);
                throw;
            }
        }

        public DicomState Print(string clientAETitle, string remoteAE, string remoteHost, int remotePort, BasicFilmSessionModuleIod basicFilmSessionModuleIod, BasicFilmBoxModuleIod basicFilmBoxModuleIod, IList<ImageBoxPixelModuleIod> imageBoxPixelModuleIods)
        {
            this._results = null;
            this._filmBoxUid = null;
            this._basicFilmSessionModuleIod = basicFilmSessionModuleIod;
            this._basicFilmBoxModuleIod = basicFilmBoxModuleIod;
            this._imageBoxPixelModuleIods = imageBoxPixelModuleIods;
            this._firstFilmBoxResponseMessage = null;
            this._currentImageBoxIndex = 0;
            base.Connect(clientAETitle, remoteAE, remoteHost, remotePort);
            return base.ResultStatus;
        }

        private void SendActionPrintRequest(DicomClient client, ClientAssociationParameters association, DicomMessage responseMessage)
        {
            DicomMessage message = new DicomMessage(null, null)
            {
                RequestedSopInstanceUid = this._imageBoxUid,
                RequestedSopClassUid = SopClass.BasicFilmBoxSopClassUid,
                ActionTypeId = 1
            };
            this._nextRequestType = RequestType.DeleteFilmBox;
            byte presentationID = association.FindAbstractSyntaxOrThrowException(SopClass.BasicGrayscalePrintManagementMetaSopClass);
            client.SendNActionRequest(presentationID, client.NextMessageID(), message);
        }

        private void SendCreateFilmBoxRequest(DicomClient client, ClientAssociationParameters association, DicomMessage responseMessage)
        {

            this._filmSessionUid = responseMessage.AffectedSopInstanceUid;
            ReferencedInstanceSequenceIod item = new ReferencedInstanceSequenceIod
            {
                ReferencedSopClassUid = SopClass.BasicFilmSessionSopClassUid,
                ReferencedSopInstanceUid = responseMessage.AffectedSopInstanceUid
            };
            this._basicFilmBoxModuleIod.ReferencedFilmSessionSequenceList.Add(item);
            DicomMessage message = new DicomMessage(null, this._basicFilmBoxModuleIod.DicomAttributeCollection);
            byte presentationID = association.FindAbstractSyntaxOrThrowException(SopClass.BasicGrayscalePrintManagementMetaSopClass);
            this._filmBoxUid = DicomUid.GenerateUid();
            this._nextRequestType = RequestType.ImageBox;
            client.SendNCreateRequest(null, presentationID, client.NextMessageID(), message, DicomUids.BasicFilmBoxSOP);
        }

        private void SendCreateFilmSessionRequest(DicomClient client, ClientAssociationParameters association)
        {
            DicomMessage message = new DicomMessage(null, this._basicFilmSessionModuleIod.DicomAttributeCollection);
            byte presentationID = association.FindAbstractSyntaxOrThrowException(SopClass.BasicGrayscalePrintManagementMetaSopClass);
            this._nextRequestType = RequestType.FilmBox;
            client.SendNCreateRequest(null, presentationID, client.NextMessageID(), message, DicomUids.BasicFilmSession);
        }

        private void SendDeleteFilmBoxRequest(DicomClient client, ClientAssociationParameters association, DicomMessage responseMessage)
        {
            DicomMessage message = new DicomMessage(null, null)
            {
                RequestedSopInstanceUid = this._imageBoxUid,
                RequestedSopClassUid = SopClass.BasicFilmBoxSopClassUid
            };
            this._nextRequestType = RequestType.DeleteFilmSession;
            byte presentationID = association.FindAbstractSyntaxOrThrowException(SopClass.BasicGrayscalePrintManagementMetaSopClass);
            client.SendNDeleteRequest(presentationID, client.NextMessageID(), message);
        }

        private void SendDeleteFilmSessionRequest(DicomClient client, ClientAssociationParameters association, DicomMessage responseMessage)
        {
            DicomMessage message = new DicomMessage(null, null)
            {
                RequestedSopInstanceUid = this._filmSessionUid,
                RequestedSopClassUid = SopClass.BasicFilmSessionSopClassUid
            };
            this._nextRequestType = RequestType.Close;
            byte presentationID = association.FindAbstractSyntaxOrThrowException(SopClass.BasicGrayscalePrintManagementMetaSopClass);
            client.SendNDeleteRequest(presentationID, client.NextMessageID(), message);
        }

        private void SendSetImageBoxRequest(DicomClient client, ClientAssociationParameters association, DicomMessage responseMessage)
        {
            if (this._currentImageBoxIndex >= this._imageBoxPixelModuleIods.Count)
            {
                this._nextRequestType = RequestType.PrintAction;
                this.SendActionPrintRequest(client, association, responseMessage);
            }
            else
            {
                if (this._currentImageBoxIndex == 0)
                {
                    this._firstFilmBoxResponseMessage = responseMessage.DataSet;
                    this._imageBoxUid = responseMessage.AffectedSopInstanceUid;
                }
                BasicFilmBoxModuleIod iod = new BasicFilmBoxModuleIod(this._firstFilmBoxResponseMessage);
                if (this._currentImageBoxIndex > iod.ReferencedImageBoxSequenceList.Count)
                {
                    throw new DicomException("Current Image Box Index is greater than number of Referenced ImageBox Sequences - set image box data");
                }
                ImageBoxPixelModuleIod iod2 = this._imageBoxPixelModuleIods[this._currentImageBoxIndex];
                DicomMessage message = new DicomMessage(null, iod2.DicomAttributeCollection)
                {
                    RequestedSopClassUid = SopClass.BasicGrayscaleImageBoxSopClassUid,
                    RequestedSopInstanceUid = iod.ReferencedImageBoxSequenceList[this._currentImageBoxIndex].ReferencedSopInstanceUid
                };
                byte presentationID = association.FindAbstractSyntax(SopClass.BasicGrayscalePrintManagementMetaSopClass);
                this._currentImageBoxIndex++;
                client.SendNSetRequest(presentationID, client.NextMessageID(), message);
            }
        }

        protected override void SetPresentationContexts()
        {
            base.AddSopClassToPresentationContext(SopClass.BasicGrayscalePrintManagementMetaSopClass);
            base.AddSopClassToPresentationContext(SopClass.PresentationLutSopClass);
        }

        // Properties
        public DicomAttributeCollection Results
        {
            get
            {
                return this._results;
            }
        }

        // Nested Types
        public delegate DicomState PrintDelegate(string clientAETitle, string remoteAE, string remoteHost, int remotePort, BasicFilmSessionModuleIod basicFilmSessionModuleIod, BasicFilmBoxModuleIod basicFilmBoxModuleIod, IList<ImageBoxPixelModuleIod> imageBoxPixelModuleIods);

        private enum RequestType
        {
            None,
            FilmSession,
            FilmBox,
            ImageBox,
            PrintAction,
            DeleteFilmBox,
            DeleteFilmSession,
            Close
        }
    }
}
