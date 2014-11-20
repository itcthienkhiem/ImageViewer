using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
    public class InteractiveHeartRatioLineGraphicBuilder : InteractiveGraphicBuilder
    {
        
		private readonly int _maximumVertices;
        private readonly int _minimumVertices;
        private int _numberOfPointsAnchored;
        private Point point;

		/// <summary>
		/// Constructs an interactive builder for the specified graphic.
		/// </summary>
		/// <param name="pointsGraphic">The graphic to be interactively built.</param>
		public InteractiveHeartRatioLineGraphicBuilder(IPointsGraphic pointsGraphic)
			: this(int.MaxValue, pointsGraphic) {}

		/// <summary>
		/// Constructs an interactive builder for the specified graphic.
		/// </summary>
		/// <param name="maximumVertices">The maximum number of vertices to accept.</param>
		/// <param name="pointsGraphic">The graphic to be interactively built.</param>
		public InteractiveHeartRatioLineGraphicBuilder(int maximumVertices, IPointsGraphic pointsGraphic)
			: this(maximumVertices, 2, pointsGraphic)
		{
		}

        /// <summary>
        /// Constructs an interactive builder for the specified graphic.
        /// </summary>
        /// <param name="maximumVertices">The maximum number of vertices to accept.</param>
        /// <param name="minimumVertices">The minimum number of vertices to accept.</param>
        /// <param name="pointsGraphic">The graphic to be interactively built.</param>
        public InteractiveHeartRatioLineGraphicBuilder(int maximumVertices, int minimumVertices, IPointsGraphic pointsGraphic)
            : base(pointsGraphic)
        {
            Platform.CheckPositive(minimumVertices, "minimumVertices");
            Platform.CheckTrue(maximumVertices >= minimumVertices, "max vertices >= min vertices");
            Platform.CheckTrue(minimumVertices > 1, "min vertices > 1");
            
            _maximumVertices = maximumVertices;
            _minimumVertices = minimumVertices;
        }

        /// <summary>
        /// Indicates whether or not to stop drawing the polyline when the user double-clicks.
        /// </summary>
        public bool StopOnDoubleClick { get; set; }

		/// <summary>
		/// Gets the graphic that the builder is operating on.
		/// </summary>
		public new IPointsGraphic Graphic
		{
			get { return (IPointsGraphic) base.Graphic; }
		}

		/// <summary>
		/// Resets any internal state of the builder, allowing the same graphic to be rebuilt.
		/// </summary>
		public override void Reset()
		{
		    /// TODO (CR Sep 2011): Shouldn't this also clear the points?
			_numberOfPointsAnchored = 0;
			base.Reset();
            point.X = point.Y = 0;
           
		}

		/// <summary>
		/// Rolls back the internal state of the builder by one mouse click, allowing the same graphic to be rebuilt by resuming from an earlier state.
		/// </summary>
		protected override void Rollback()
		{
            /// TODO (CR Sep 2011): Shouldn't this also remove the last point?
            _numberOfPointsAnchored = Math.Max(_numberOfPointsAnchored - 1, 0);
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Start"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the builder did something as a result of the call, and hence would like to receive capture; False otherwise.</returns>
		public override bool Start(IMouseInformation mouseInformation)
		{
            if (mouseInformation.ClickCount == 2 && StopOnDoubleClick && _numberOfPointsAnchored >= _minimumVertices)
            {
                NotifyGraphicComplete();
                return true;
            }

			_numberOfPointsAnchored++;

			// We just started creating
			if (_numberOfPointsAnchored == 1)
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				this.Graphic.Points.Add(mouseInformation.Location);
                this.Graphic.Points.Add(mouseInformation.Location);
                this.Graphic.ResetCoordinateSystem();
                point = mouseInformation.Location;
			}
			// We're done creating
			else if (_numberOfPointsAnchored == _maximumVertices)
			{
                // When user moves very quickly and events are filtered for performance purpose (eg web viewer case), 
                // the final point may not be the same as the last tracked point. Must update the final point based on the latest mouse position.

                this.Graphic.CoordinateSystem = CoordinateSystem.Destination;

			    //this.Graphic.Points[this.Graphic.Points.Count-1] = mouseInformation.Location;
                this.Graphic.Points[this.Graphic.Points.Count - 1] = new Point(mouseInformation.Location.X, point.Y);
                this.Graphic.ResetCoordinateSystem();

				this.NotifyGraphicComplete();
			}
			// We're in the middle of creating
			else
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;

                // Update the final position of current point based on the latest mouse position.
                //this.Graphic.Points[Graphic.Points.Count-1] = mouseInformation.Location;
                this.Graphic.Points[this.Graphic.Points.Count - 1] = new Point(mouseInformation.Location.X, point.Y);

                // Add a new point for tracking
                this.Graphic.Points.Add(mouseInformation.Location);
				this.Graphic.ResetCoordinateSystem();
			}

			return true;
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Track"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the builder handled the message; False otherwise.</returns>
		public override bool Track(IMouseInformation mouseInformation)
		{
			this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
			//this.Graphic.Points[_numberOfPointsAnchored] = mouseInformation.Location;
            this.Graphic.Points[_numberOfPointsAnchored] = new Point(mouseInformation.Location.X, point.Y);
			this.Graphic.ResetCoordinateSystem();
			this.Graphic.Draw();

			return true;
		}

		/// <summary>
		/// Passes user input to the builder when <see cref="IMouseButtonHandler.Stop"/> is called on the owning tool.
		/// </summary>
		/// <param name="mouseInformation">The user input data.</param>
		/// <returns>True if the tool should not release capture; False otherwise.</returns>
		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}
    }
}
