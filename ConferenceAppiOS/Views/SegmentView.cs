using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using CoreAnimation;
using Foundation;

namespace ConferenceAppiOS
{
	public class SegmentView : UIControl
	{ 
		public enum SGSegmentedControlSelectionStyle{
			TextWidthStripe,  // Indicator width will only be as big as the text width
			FullWidthStripe, // Indicator width will fill the whole segment
			Box, // A rectangle that covers the whole segment
			Arrow // An arrow in the middle of the segment pointing up or down depending on `HMSegmentedControlSelectionIndicatorLocation`
		}

		public enum SGSegmentedControlSelectionIndicatorLocation{
			Up, 
			Down, 
			None // No selection indicator
		}

		public enum SGSegmentedControlSegmentWidthStyle{
			Fixed, // Segment width is fixed
			Dynamic, // Segment width will only be as big as the text width (including inset)
		}
		public enum SGSegmentedControlType{
			Text,
			Images, 
			TextImages
		}

		private List<string> _sectionTitles = new List<string>();
		public List<string> sectionTitles {
			get{
				return _sectionTitles;
			}
			set {
				_sectionTitles = value;
				this.SetNeedsLayout ();
			}
		}

		private List<UIImage> _sectionImages = new List<UIImage>();
		public List<UIImage> sectionImages {
			get{
				return _sectionImages;
			}
			set{
				_sectionImages = value;
				this.SetNeedsLayout ();
			}
		}

		public static nfloat segmentImageTextPadding = 7;
		public List<UIImage> sectionSelectedImages;

		public bool multiLineSupport;
		public UIFont multiLineFont;

		public delegate void SegmentViewViewDelegate(int index);

		public event SegmentViewViewDelegate segmentViewViewDelegate;

        public Action<int> SegmentChangedHandler;

		public UIFont font;

		public UIColor textColor;

		public UIColor subHeadingTextColor;

		public UIColor selectedTextColor;

		public UIColor backgroundColor;

		public UIColor selectionIndicatorColor;

		public UIColor separatorColor;

		public UIColor selectedBoxColor;

		public nfloat selectedBoxColorOpacity;

		public nfloat selectedBoxBorderWidth;

		public UIColor selectedBoxBorderColor;

		private SGSegmentedControlType _type;
		public SGSegmentedControlType type {
			get{
				return _type;
			}
			set{
				_type = value;
			}
		}
		private SGSegmentedControlSelectionStyle _selectionStyle;
		public SGSegmentedControlSelectionStyle selectionStyle {
			get{
				return _selectionStyle;
			}
			set{
				_selectionStyle = value;
			}
		}

		private SGSegmentedControlSegmentWidthStyle _segmentWidthStyle;
		public SGSegmentedControlSegmentWidthStyle segmentWidthStyle {
			get{
				return _segmentWidthStyle;
			}
			set{
				_segmentWidthStyle = value;
			}
		}

		public SGSegmentedControlSelectionIndicatorLocation _selectionIndicatorLocation;

		public SGSegmentedControlSelectionIndicatorLocation selectionIndicatorLocation {
			get{
				return _selectionIndicatorLocation;
			}
			set{
				_selectionIndicatorLocation = value;
				if(value == SGSegmentedControlSelectionIndicatorLocation.None)
					this.selectionIndicatorHeight = 0.0f;
			}
		}



		public bool scrollEnabled;

		public bool userDraggable;

		public bool touchEnabled;

		public int selectedSegmentIndex;

		public nfloat selectionIndicatorHeight;

		public UIEdgeInsets segmentEdgeInset;

		public bool shouldAnimateUserSelection;

		CALayer selectionIndicatorStripLayer;
		CALayer selectionIndicatorBoxLayer;
		CALayer selectionIndicatorArrowLayer;
		private nfloat segmentWidth;
		private List<nfloat> segmentWidthsArray;
		private SGScrollView scrollView;


		public SegmentView (CGRect frame){
			base.Frame = frame;
			commonInit ();
		}
		public SegmentView (List<string>sectiontitles)
		{
			commonInit ();
			this.sectionTitles = sectiontitles;
			this.type = SGSegmentedControlType.Text;
		}

		public SegmentView (List<UIImage>sectionImages, List<UIImage>sectionSelectedImages)
		{
			commonInit ();
			this.sectionImages = sectionImages;
			this.sectionSelectedImages = sectionSelectedImages;
			this.type = SGSegmentedControlType.Images;
		}

		public SegmentView (List<UIImage>sectionImages, List<UIImage>sectionSelectedImages, List<string> sectiontitles)
		{
			commonInit ();
			if (sectionImages.Count != sectiontitles.Count) {
				 
			}
			this.sectionImages = sectionImages;
			this.sectionSelectedImages = sectionSelectedImages;
			this.sectionTitles = sectiontitles;
			this.type = SGSegmentedControlType.TextImages;
		}

		void commonInit(){

			this.scrollView = new SGScrollView(new CGRect(0,0,0,0));
			this.scrollView.ScrolledToTop += (s, e) => {};
			this.scrollView.ShowsHorizontalScrollIndicator = false;
			this.scrollView.ShowsVerticalScrollIndicator = false;
			this.AddSubview (this.scrollView);

			this.font = UIFont.FromName (@"Gotham-Bold", 18.0f);

			this.textColor = UIColor.Black;
			this.subHeadingTextColor = UIColor.Black;
			this.selectedTextColor = UIColor.Black;
			this.backgroundColor = UIColor.White;
			this.Opaque = false;
			this.selectionIndicatorColor = UIColor.FromRGBA(52.0f / 255.0f, 181.0f / 255.0f, 229.0f / 255.0f, 1.0f);
			this.selectedSegmentIndex = 0;
			this.segmentEdgeInset = new UIEdgeInsets(0, 5, 0, 5);
			this.selectionIndicatorHeight = 5.0f;
			this.selectionStyle = SGSegmentedControlSelectionStyle.TextWidthStripe;
			this.selectionIndicatorLocation = SGSegmentedControlSelectionIndicatorLocation.Up;
			this.segmentWidthStyle = SGSegmentedControlSegmentWidthStyle.Fixed;
			this.userDraggable = true;
			this.touchEnabled = true;
			this.type = SGSegmentedControlType.Text;
			this.selectedBoxColor = UIColor.White;
			this.separatorColor = UIColor.FromRGB (0, 0, 128);
			this.selectedBoxColorOpacity = 0.2f;
			this.selectedBoxBorderWidth = 1.0f;
			this.selectedBoxBorderColor = UIColor.FromRGBA (0.0f, 0.0f, 0.0f, 0.5f);
			this.shouldAnimateUserSelection = true;

			this.selectionIndicatorArrowLayer = new CALayer();

			this.selectionIndicatorStripLayer = new CALayer();

			this.selectionIndicatorBoxLayer = new CALayer();

			this.ContentMode = UIViewContentMode.Redraw;

		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			updateSegmentsRects ();
			SetNeedsDisplay ();
		}
		public override void Draw (CGRect rectngle)
		{
			base.Draw (rectngle);

			this.backgroundColor.SetFill ();	

			this.selectionIndicatorBoxLayer.BackgroundColor = this.selectedBoxColor.CGColor;
			this.selectionIndicatorBoxLayer.Opacity = (float)this.selectedBoxColorOpacity;
			this.selectionIndicatorBoxLayer.BorderWidth = this.selectedBoxBorderWidth;
			this.selectionIndicatorBoxLayer.BorderColor = this.selectedBoxBorderColor.CGColor;

			this.selectionIndicatorArrowLayer.BackgroundColor = this.selectionIndicatorColor.CGColor;
			this.selectionIndicatorStripLayer.BackgroundColor = this.selectionIndicatorColor.CGColor;
			if (this.scrollView.Layer.Sublayers != null) {
				foreach (CALayer layer in this.scrollView.Layer.Sublayers) {
					layer.RemoveFromSuperLayer ();
				}
			}
			if (this.selectedSegmentIndex != -1) { 
				if (this.selectionStyle == SGSegmentedControlSelectionStyle.Arrow) {
					if (this.selectionIndicatorArrowLayer.SuperLayer == null) {
						this.setArrowFrame();
						this.scrollView.Layer.AddSublayer(this.selectionIndicatorArrowLayer);
					}
				} else {
					if (this.selectionIndicatorStripLayer.SuperLayer == null) {
						this.selectionIndicatorStripLayer.Frame = frameForSelectionIndicator();
						this.scrollView.Layer.AddSublayer (this.selectionIndicatorStripLayer);

						if (this.selectionStyle == SGSegmentedControlSelectionStyle.Box && this.selectionIndicatorBoxLayer.SuperLayer == null) {
							this.selectionIndicatorBoxLayer.Frame = frameForFillerSelectionIndicator();
							this.scrollView.Layer.InsertSublayer (this.selectionIndicatorBoxLayer, 0);
						}
					}
				}
			}

			if (this.type == SGSegmentedControlType.Text) {

				int idx = 0;
				foreach(var titleString in this.sectionTitles){
					if (this.multiLineSupport) {
						string[] stringArray = titleString.Split (',');
						NSString titleString1 = new NSString("");
						NSString titleString2 = new NSString("");
						if (stringArray.Length > 0) {
							titleString1 = new NSString(stringArray[0].ToUpper());
						}
						if (stringArray.Length > 1) {
							titleString2 = new NSString(stringArray[1]);
						}

						nfloat stringHeight1 = titleString1.StringSize (this.font).Height;

						nfloat stringWidth1 = titleString1.StringSize(this.font).Width;

						nfloat stringHeight2 = titleString2.StringSize(this.multiLineFont).Height+2; 
						nfloat stringWidth2 = titleString2.StringSize(this.multiLineFont).Width;

					
						nfloat y = 0;
						if((this.selectionIndicatorLocation == SGSegmentedControlSelectionIndicatorLocation.Up)){
							y = this.Frame.Height / 2 - (stringHeight1 + stringHeight2) / 2 + this.selectionIndicatorHeight;
						}else{
							y = this.Frame.Height / 2 - (stringHeight1 + stringHeight2) / 2;
						}

						CGRect rect = new CGRect(0,0,0,0);
						if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Fixed) {
							rect = new CGRect((this.segmentWidth * idx) + (this.segmentWidth - stringWidth1)/2, y, stringWidth1, stringHeight1);
						} else if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Dynamic) {
							
							nfloat xOffset = 0;
							int i = 0;
							foreach (nfloat width in this.segmentWidthsArray) {
								if (idx == i)
									break;
								xOffset = xOffset + width;
								i++;
							}
							rect = new CGRect (xOffset, y, this.segmentWidthsArray[idx],stringHeight1);
						}

						CATextLayer titleLayer = new CATextLayer();
						titleLayer.Frame = rect;
						titleLayer.SetFont(this.font.Name);
						titleLayer.FontSize = this.font.PointSize;
						titleLayer.AlignmentMode = CATextLayer.AlignmentCenter;
						titleLayer.String = titleString1;
						titleLayer.TruncationMode = CATextLayer.TruncantionEnd;

						if (this.selectedSegmentIndex == idx) {
							titleLayer.ForegroundColor = this.selectedTextColor.CGColor;
						} else {
							titleLayer.ForegroundColor = this.textColor.CGColor;
						}

						titleLayer.ContentsScale = UIScreen.MainScreen.Scale;
						this.scrollView.Layer.AddSublayer(titleLayer);

						titleLayer.ContentsScale = UIScreen.MainScreen.Scale;
						this.scrollView.Layer.AddSublayer(titleLayer);
						y = titleLayer.Frame.Y + titleLayer.Frame.Height + 2;

						titleLayer = new CATextLayer();
						titleLayer.Frame = new CGRect((this.segmentWidth*idx)+(this.segmentWidth - stringWidth2)/2,y,stringWidth2, stringHeight2);
						titleLayer.SetFont(this.multiLineFont.Name);
						titleLayer.FontSize = this.multiLineFont.PointSize;
						titleLayer.AlignmentMode = CATextLayer.AlignmentCenter;
						titleLayer.String = titleString2;
						titleLayer.TruncationMode = CATextLayer.TruncantionEnd;

						if (this.selectedSegmentIndex == idx) {
							titleLayer.ForegroundColor = this.selectedTextColor.CGColor;
						} else {
							titleLayer.ForegroundColor = this.subHeadingTextColor.CGColor;
						}

						titleLayer.ContentsScale = UIScreen.MainScreen.Scale;
						this.scrollView.Layer.AddSublayer(titleLayer);

					}else {
						NSString titleStr = new NSString(titleString.ToUpper());

						nfloat stringHeight = titleStr.StringSize(this.font).Height;
						nfloat stringWidth = titleStr.StringSize(this.font).Width;

						nfloat y = 0;
						if(this.selectionIndicatorLocation == SGSegmentedControlSelectionIndicatorLocation.Up){
							y = this.Frame.Height/2- stringHeight/2 + this.selectionIndicatorHeight;
						}else{
							y = this.Frame.Height/2- stringHeight/2;
						}

						CGRect rect = new CGRect(0,0,0,0);
						if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Fixed) {
							rect = new CGRect(this.segmentWidth * idx + (this.segmentWidth - stringWidth)/2,y,stringWidth,stringHeight+10);
						} else if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Dynamic) {
							nfloat xOffset = 0;
							int i = 0;
							foreach (int width in this.segmentWidthsArray) {
								if (idx == i)
									break;
								xOffset = xOffset + width;
								i++;
							}

							rect = new CGRect(xOffset,y,this.segmentWidthsArray[idx],stringHeight);
						}

						CATextLayer titleLayer = new CATextLayer();
						titleLayer.Frame = rect;
						titleLayer.SetFont(this.font.Name);
						titleLayer.FontSize = this.font.PointSize;
						titleLayer.AlignmentMode = CATextLayer.AlignmentCenter;
						titleLayer.String = titleStr;
						titleLayer.TruncationMode = CATextLayer.TruncantionEnd;
						if (this.selectedSegmentIndex == idx) {
							titleLayer.ForegroundColor = this.selectedTextColor.CGColor;
						} else {
							titleLayer.ForegroundColor = this.textColor.CGColor;
						}

						titleLayer.ContentsScale = UIScreen.MainScreen.Scale;
						this.scrollView.Layer.AddSublayer(titleLayer); 
					}
					if (this.sectionTitles.Count-1 != idx) {
						CALayer layer = new CALayer();
						layer.Frame = new CGRect((this.segmentWidth * (idx+1)), 0, 1, this.Frame.Height);
						layer.BackgroundColor = separatorColor.CGColor;
						this.scrollView.Layer.AddSublayer(layer);
					}
					
					idx++;
				}
			} else if (this.type == SGSegmentedControlType.Images) {

				int idx = 0;
				foreach(UIImage iconImage in this.sectionImages){
					UIImage icn = iconImage;
					nfloat imageWidth = icn.Size.Width;
					nfloat imageHeight = icn.Size.Height;
					nfloat y = 0;
					if(this.selectionIndicatorLocation == SGSegmentedControlSelectionIndicatorLocation.Up){
						y = this.Frame.Height - this.selectionIndicatorHeight / 2 - imageHeight / 2 + this.selectionIndicatorHeight;
					}else{
						y = this.Frame.Height - this.selectionIndicatorHeight / 2 - imageHeight / 2;
					}

					nfloat x = this.segmentWidth * idx + (this.segmentWidth - imageWidth)/2.0f;
					CGRect rect = new CGRect(x, y, imageWidth, imageHeight);

					CALayer imageLayer = new CALayer();
					imageLayer.Frame = rect;

					if (this.selectedSegmentIndex == idx) {
						if (this.sectionSelectedImages != null) {
							UIImage highlightIcon = this.sectionSelectedImages[idx];
							imageLayer.Contents = highlightIcon.CGImage;
						} else {
							imageLayer.Contents = icn.CGImage;
						}
					} else {
						imageLayer.Contents = icn.CGImage;
					}

					this.scrollView.Layer.AddSublayer(imageLayer);
					idx++;
				}
			} else if (this.type == SGSegmentedControlType.TextImages){

				int idx = 0;
				foreach(UIImage iconImage in this.sectionImages){
					UIImage icon = iconImage;
					nfloat imageWidth = icon.Size.Width;
					nfloat imageHeight = icon.Size.Height;
					NSString titls = new NSString(this.sectionTitles[idx]);
					nfloat stringHeight = titls.StringSize(this.font).Height;
					nfloat yOffset = 0;
					if(this.selectionIndicatorLocation == SGSegmentedControlSelectionIndicatorLocation.Up){
						yOffset = this.Frame.Height - this.selectionIndicatorHeight/2 - stringHeight/2 + this.selectionIndicatorHeight;
					}else{
						yOffset = this.Frame.Height - this.selectionIndicatorHeight/2 - stringHeight/2;
					}
					nfloat imageXOffset = this.segmentEdgeInset.Left;

					if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Fixed)
						imageXOffset = this.segmentWidth * idx;
					else if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Dynamic) {
						
						int i = 0;
						foreach (nfloat width in this.segmentWidthsArray) {
							if (idx == i)
								break;
							imageXOffset = imageXOffset + width;
							i++;
						}
					}

					CGRect imageRect = new CGRect(imageXOffset, yOffset, imageWidth, imageHeight);

					
					nfloat textXOffset = imageXOffset + imageWidth + segmentImageTextPadding;

					CGRect textRect = new CGRect(textXOffset, yOffset, this.segmentWidthsArray[idx]-imageWidth-segmentImageTextPadding-this.segmentEdgeInset.Left-this.segmentEdgeInset.Right, stringHeight);
					CATextLayer titleLayer = new CATextLayer();
					titleLayer.Frame = textRect;
					titleLayer.SetFont(this.font.Name);
					titleLayer.FontSize = this.font.PointSize;
					titleLayer.AlignmentMode = CATextLayer.AlignmentCenter;
					titleLayer.String = this.sectionTitles[idx];
					titleLayer.TruncationMode = CATextLayer.TruncantionEnd;

					CALayer imageLayer = new CALayer();
					imageLayer.Frame = imageRect;

					if (this.selectedSegmentIndex == idx) {
						if (this.sectionSelectedImages != null) {
							UIImage highlightIcon = this.sectionSelectedImages[idx];
							imageLayer.Contents = highlightIcon.CGImage;
						} else {
							imageLayer.Contents = icon.CGImage;
						}
						titleLayer.ForegroundColor = this.selectedTextColor.CGColor;
					} else {
						imageLayer.Contents = icon.CGImage;
						titleLayer.ForegroundColor = this.textColor.CGColor;
					}

					this.scrollView.Layer.AddSublayer(imageLayer);
					titleLayer.ContentsScale = UIScreen.MainScreen.Scale;
					this.scrollView.Layer.AddSublayer(titleLayer);

				}
				idx++;
			}
		}	

		void setArrowFrame() {

			this.selectionIndicatorArrowLayer.Frame = frameForSelectionIndicator();

			this.selectionIndicatorArrowLayer.Mask = null;

			UIBezierPath arrowPath = new UIBezierPath();

			CGPoint p1 = CGPoint.Empty;
			CGPoint p2 = CGPoint.Empty;
			CGPoint p3 = CGPoint.Empty;

			if (this.selectionIndicatorLocation == SGSegmentedControlSelectionIndicatorLocation.Down) {
				p1 = new CGPoint(this.selectionIndicatorArrowLayer.Bounds.Size.Width / 2, 0);
				p2 = new CGPoint(0, this.selectionIndicatorArrowLayer.Bounds.Size.Height);
				p3 = new CGPoint(this.selectionIndicatorArrowLayer.Bounds.Size.Width, this.selectionIndicatorArrowLayer.Bounds.Size.Height);
			}

			if (this.selectionIndicatorLocation == SGSegmentedControlSelectionIndicatorLocation.Up) {
				p1 = new CGPoint(this.selectionIndicatorArrowLayer.Bounds.Size.Width / 2, this.selectionIndicatorArrowLayer.Bounds.Size.Height);
				p2 = new CGPoint(this.selectionIndicatorArrowLayer.Bounds.Size.Width, 0);
				p3 = new CGPoint(0, 0);
			}

			arrowPath.MoveTo (p1);
			arrowPath.AddLineTo (p2);
			arrowPath.AddLineTo (p3);
			arrowPath.ClosePath ();

			CAShapeLayer maskLayer = new CAShapeLayer();
			maskLayer.Frame = this.selectionIndicatorArrowLayer.Bounds;
			maskLayer.Path = arrowPath.CGPath;
			this.selectionIndicatorArrowLayer.Mask = maskLayer;
		}



		public CGRect frameForFillerSelectionIndicator() {
			if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Dynamic) {
				nfloat selectedSegmentOffset = 0.0f;
				int i = 0;
				foreach (nfloat width in this.segmentWidthsArray) {
					if (this.selectedSegmentIndex == i) {
						break;					
					}

					selectedSegmentOffset = selectedSegmentOffset + width;
					i++;
				}

				return new CGRect(selectedSegmentOffset, 0, this.segmentWidthsArray[this.selectedSegmentIndex] , this.Frame.Height);
			}
			return new CGRect(this.segmentWidth * this.selectedSegmentIndex, 0, this.segmentWidth, this.Frame.Height);
		}



		public CGRect frameForSelectionIndicator() {
			nfloat indicatorYOffset = 0.0f;

			if (this.selectionIndicatorLocation == SGSegmentedControlSelectionIndicatorLocation.Down) {
				indicatorYOffset = this.Bounds.Size.Height - this.selectionIndicatorHeight;			
			}


			nfloat sectionWidth = 0.0f;

			if (this.type == SGSegmentedControlType.Text) {
				NSString str = new NSString (this.sectionTitles[this.selectedSegmentIndex]);
				nfloat stringWidth =  str.StringSize(this.font).Width;
				sectionWidth = stringWidth;
			} else if (this.type == SGSegmentedControlType.Images) {
				UIImage sectionImage = this.sectionImages[this.selectedSegmentIndex];
				nfloat imageWidth = sectionImage.Size.Width;
				sectionWidth = imageWidth;
			} else if (this.type == SGSegmentedControlType.TextImages) {
				NSString str = new NSString (this.sectionTitles [this.selectedSegmentIndex]);
				nfloat stringWidth =  str.StringSize(this.font).Width;
				UIImage sectionImage = this.sectionImages[this.selectedSegmentIndex];
				nfloat imageWidth = sectionImage.Size.Width;
				if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Fixed) {
					sectionWidth = (nfloat)Math.Max(stringWidth, imageWidth);
				} else if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Dynamic) {
					sectionWidth = imageWidth + segmentImageTextPadding + stringWidth;
				}
			}

			if (this.selectionStyle == SGSegmentedControlSelectionStyle.Arrow) {
				nfloat widthToEndOfSelectedSegment = (this.segmentWidth * this.selectedSegmentIndex) + this.segmentWidth;
				nfloat widthToStartOfSelectedIndex = (this.segmentWidth * this.selectedSegmentIndex);

				nfloat x = widthToStartOfSelectedIndex + ((widthToEndOfSelectedSegment - widthToStartOfSelectedIndex) / 2) - (this.selectionIndicatorHeight/2);
				return new CGRect(x, indicatorYOffset, this.selectionIndicatorHeight, this.selectionIndicatorHeight);
			} else {
				if (this.selectionStyle == SGSegmentedControlSelectionStyle.TextWidthStripe &&
					sectionWidth <= this.segmentWidth &&
					this.segmentWidthStyle != SGSegmentedControlSegmentWidthStyle.Dynamic) {
					nfloat widthToEndOfSelectedSegment = (this.segmentWidth * this.selectedSegmentIndex) + this.segmentWidth;
					nfloat widthToStartOfSelectedIndex = (this.segmentWidth * this.selectedSegmentIndex);

					nfloat x = ((widthToEndOfSelectedSegment - widthToStartOfSelectedIndex) / 2) + (widthToStartOfSelectedIndex - sectionWidth / 2);
					return new CGRect(x, indicatorYOffset, sectionWidth, this.selectionIndicatorHeight);
				} else {
					if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Dynamic) {
						nfloat selectedSegmentOffset = 0.0f;

						int i = 0;
						foreach (nfloat width in this.segmentWidthsArray) {
							if (this.selectedSegmentIndex == i)
								break;
							selectedSegmentOffset = selectedSegmentOffset + width;
							i++;
						}

						return new CGRect(selectedSegmentOffset, indicatorYOffset, this.segmentWidthsArray[this.selectedSegmentIndex], this.selectionIndicatorHeight);
					}

					return new CGRect(this.segmentWidth * this.selectedSegmentIndex, indicatorYOffset, this.segmentWidth, this.selectionIndicatorHeight);
				}
			}
		}

		void updateSegmentsRects() {

			this.scrollView.Frame = new CGRect(0, 0, this.Frame.Width,this.Frame.Height);

			if (this.sectionCount() > 0) {
				this.segmentWidth = this.Frame.Size.Width /  sectionCount(); 
			}

			if (this.type == SGSegmentedControlType.Text && this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Fixed) {
				foreach (string titleString in this.sectionTitles) {
					NSString str = new NSString (titleString);
					nfloat stringWidth = str.WeakGetSizeUsingAttributes(NSDictionary.FromObjectAndKey(UIStringAttributeKey.Font, this.font)).Width + this.segmentEdgeInset.Left + this.segmentEdgeInset.Right;
					this.segmentWidth = (nfloat)((multiLineSupport && this.sectionTitles.Count > 4) ? 80 : Math.Max(stringWidth, this.segmentWidth));
				}
			} else if (this.type == SGSegmentedControlType.Text && this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Dynamic) {
				List<nfloat> mutableSegmentWidths = new List<nfloat>();

				foreach (string titleString in this.sectionTitles) {
					NSString str = new NSString (titleString);
					nfloat stringWidth = str.WeakGetSizeUsingAttributes(NSDictionary.FromObjectAndKey(UIStringAttributeKey.Font, this.font)).Width + this.segmentEdgeInset.Left + this.segmentEdgeInset.Right;
					mutableSegmentWidths.Add (stringWidth);
				}
				this.segmentWidthsArray = mutableSegmentWidths;

			} else if (this.type == SGSegmentedControlType.Images) {
				foreach (UIImage sectionImage in this.sectionImages) {
					nfloat imageWidth = sectionImage.Size.Width + this.segmentEdgeInset.Left + this.segmentEdgeInset.Right;
					this.segmentWidth = (nfloat)Math.Max(imageWidth, this.segmentWidth);
				}
			} else if (this.type == SGSegmentedControlType.TextImages && this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Fixed){
			
				foreach (string titleString in this.sectionTitles) {
					NSString str = new NSString(titleString);
					nfloat stringWidth = str.WeakGetSizeUsingAttributes(NSDictionary.FromObjectAndKey(UIStringAttributeKey.Font, this.font)).Width + this.segmentEdgeInset.Left + this.segmentEdgeInset.Right; 
					this.segmentWidth = (nfloat)Math.Max(stringWidth, this.segmentWidth);
				}
			} else if (this.type == SGSegmentedControlType.TextImages && this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Dynamic) {
				List<nfloat> mutableSegmentWidths = new List<nfloat>();
				int i = 0;
				foreach (string titleString in this.sectionTitles) {
					NSString str = new NSString(titleString);
					nfloat stringWidth = str.WeakGetSizeUsingAttributes(NSDictionary.FromObjectAndKey(UIStringAttributeKey.Font, this.font)).Width + this.segmentEdgeInset.Right;
					UIImage sectionImage = this.sectionImages[i];
					nfloat imageWidth = sectionImage.Size.Width + this.segmentEdgeInset.Left;

					nfloat combinedWidth = imageWidth + segmentImageTextPadding + stringWidth;

					mutableSegmentWidths.Add(combinedWidth);

					i++;
				}
				this.segmentWidthsArray = mutableSegmentWidths;
			}

			this.scrollView.ScrollEnabled = this.userDraggable;
			this.scrollView.ContentSize = new CGSize(totalSegmentedControlWidth(), this.Frame.Size.Height);
		}


		public int sectionCount() {
			if (this.type == SGSegmentedControlType.Text) {
				return this.sectionTitles.Count;
			} else if (this.type == SGSegmentedControlType.Images ||
				this.type == SGSegmentedControlType.TextImages) {
				return this.sectionImages.Count;
			}
			return 0;
		}


		void willMoveToSuperview(UIView newSuperview) {
			if (newSuperview == null)
				return;

			if (this.sectionTitles != null || this.sectionImages != null) {
				updateSegmentsRects ();
			}
		}


		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			UITouch touch = touches.AnyObject as UITouch;
			CGPoint touchLocation = touch.LocationInView(this);

			if (this.Bounds.Contains(touchLocation)) {
				int segment = 0;
				if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Fixed) {
					nfloat point = (touchLocation.X + this.scrollView.ContentOffset.X) / this.segmentWidth;
					segment = Convert.ToInt32(Math.Floor(Convert.ToDouble(point)));
				} else if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Dynamic) {
					
					nfloat widthLeft = (touchLocation.X + this.scrollView.ContentOffset.X);
					foreach (nfloat width in this.segmentWidthsArray) {
						widthLeft = widthLeft - width;
						if (widthLeft <= 0)
							break; 

						segment++;
					}
				}

				if (segment != this.selectedSegmentIndex && segment < this.sectionTitles.Count) {
					if (this.touchEnabled)
						setSelectedSegmentIndex(segment,this.shouldAnimateUserSelection, true);
				}
			}
		}


		public nfloat totalSegmentedControlWidth() {
			if (this.type == SGSegmentedControlType.Text && this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Fixed) {
				return this.sectionTitles.Count * this.segmentWidth;
			} else if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Dynamic) {
				return 0;
			} else {
				return this.sectionImages.Count * this.segmentWidth;
			}
		}

		public void scrollToSelectedSegmentIndex() {
			CGRect rectForSelectedIndex = new CGRect(0,0,0,0);
			nfloat selectedSegmentOffset = 0;
			if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Fixed) {
				rectForSelectedIndex = new CGRect(this.segmentWidth * this.selectedSegmentIndex,
					0,
					this.segmentWidth,
					this.Frame.Size.Height);

				selectedSegmentOffset = (this.Frame.Width / 2) - (this.segmentWidth / 2);
			} else if (this.segmentWidthStyle == SGSegmentedControlSegmentWidthStyle.Dynamic) {
				int i = 0;
				nfloat offsetter = 0;
				foreach (nfloat width in this.segmentWidthsArray) {
					if (this.selectedSegmentIndex == i)
						break;
					offsetter = offsetter + width;
					i++;
				}

				rectForSelectedIndex = new CGRect(offsetter,
					0,
					this.segmentWidthsArray[this.selectedSegmentIndex],
					this.Frame.Size.Height);

				selectedSegmentOffset = ((this.Frame.Size.Width) / 2) - (this.segmentWidthsArray[this.selectedSegmentIndex] / 2);
			}

			CGRect rectToScrollTo = rectForSelectedIndex;
			rectToScrollTo.X -= selectedSegmentOffset;
			var newSize = rectToScrollTo.Size;
			newSize.Width = selectedSegmentOffset * 2;
			rectToScrollTo.Size = newSize;
			this.scrollView.ScrollRectToVisible(rectToScrollTo, true);
		}


		public void setSelectedSegmentIndex(int index) {
			setSelectedSegmentIndex(index,false,false);
		}

		public void setSelectedSegmentIndex(int index, bool animated){
			setSelectedSegmentIndex(index,animated,false);
		}

		public void setSelectedSegmentIndex(int index, bool animated, bool notify){
			this.selectedSegmentIndex = index;
			SetNeedsDisplay();
			if (index == -1) {
				this.selectionIndicatorArrowLayer.RemoveFromSuperLayer();
				this.selectionIndicatorStripLayer.RemoveFromSuperLayer();
				this.selectionIndicatorBoxLayer.RemoveFromSuperLayer();
			} else {
				scrollToSelectedSegmentIndex();

				if (animated) {
					if(this.selectionStyle == SGSegmentedControlSelectionStyle.Arrow) {
						if (this.selectionIndicatorArrowLayer.SuperLayer == null) {
							this.scrollView.Layer.AddSublayer(this.selectionIndicatorArrowLayer);
							setSelectedSegmentIndex(index, false, true);
							return;
						}
					}else {
						if (this.selectionIndicatorStripLayer.SuperLayer == null) {
							this.scrollView.Layer.AddSublayer(this.selectionIndicatorStripLayer);
							if (this.selectionStyle == SGSegmentedControlSelectionStyle.Box && this.selectionIndicatorBoxLayer.SuperLayer == null)
								this.scrollView.Layer.InsertSublayer(this.selectionIndicatorBoxLayer,0);

							setSelectedSegmentIndex(index,false,true);
							return;
						}
					}

					if (notify)
						notifyForSegmentChangeToIndex(index);

					
					if(this.selectionIndicatorArrowLayer.Actions != null)
						this.selectionIndicatorArrowLayer.Actions = null;

					if(this.selectionIndicatorStripLayer.Actions != null)
						this.selectionIndicatorStripLayer.Actions = null;

					if(this.selectionIndicatorBoxLayer.Actions != null)
						this.selectionIndicatorBoxLayer.Actions = null;

					CATransaction.Begin();
					CATransaction.AnimationDuration = 0.15;
					CAMediaTimingFunction fn = new CAMediaTimingFunction(0,0,0,0);
					CATransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);
					
					setArrowFrame();
					this.selectionIndicatorBoxLayer.Frame = frameForSelectionIndicator();//[self frameForSelectionIndicator];
					this.selectionIndicatorStripLayer.Frame = frameForSelectionIndicator();
					this.selectionIndicatorBoxLayer.Frame = frameForFillerSelectionIndicator();

					CATransaction.Commit();

				} else {

					
					NSObject[] values = new NSObject[]
					{
						null, 
						null
					};

					
					NSObject[] keys = new NSObject[]
					{
						new NSString("Position"),
						new NSString("Bounds")
					};

					NSMutableDictionary newActions =  NSMutableDictionary.FromObjectsAndKeys(values,keys); 
					this.selectionIndicatorArrowLayer.Actions = newActions;
					setArrowFrame();

					this.selectionIndicatorStripLayer.Actions = newActions;
					this.selectionIndicatorStripLayer.Frame = frameForSelectionIndicator();

					this.selectionIndicatorBoxLayer.Actions = newActions;
					this.selectionIndicatorBoxLayer.Frame = frameForFillerSelectionIndicator();

					if (notify)
						notifyForSegmentChangeToIndex(index);
				}
			}

		}

		public void notifyForSegmentChangeToIndex(int index){
			if(this.Superview != null)
				SendActionForControlEvents(UIControlEvent.ValueChanged);

			if(segmentViewViewDelegate != null)
				segmentViewViewDelegate(index);

            if (SegmentChangedHandler != null)
                SegmentChangedHandler(index);

		}

	}

	public class SGScrollView : UIScrollView{

		public SGScrollView(CGRect rect) : base(rect){

		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			if (this.Dragging == false) {
				this.NextResponder.TouchesBegan (touches, evt);
			}else{
				base.TouchesBegan (touches, evt);
			}
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			if (this.Dragging == false) {
				this.NextResponder.TouchesMoved (touches, evt);
			} else {
				base.TouchesMoved (touches, evt);
			}
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			if (this.Dragging == false) {
				this.NextResponder.TouchesEnded (touches, evt);
			} else {
				base.TouchesEnded (touches, evt);
			}
		}
	}
}
