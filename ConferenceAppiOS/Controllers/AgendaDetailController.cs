using System;
using CoreGraphics;
using CoreFoundation;
using UIKit;
using Foundation;
using CoreData;
using CommonLayer.Entities.Built;
using SQLiteNetExtensions.Extensions;
using System.Linq;
using CoreAnimation;
using System.Collections.Generic;
using ConferenceAppiOS.Helpers;
using CommonLayer;
using Newtonsoft.Json;

namespace ConferenceAppiOS
{
    public class AgendaDetailController : BaseViewController
    {
        BuiltAgendaItem _builtAgendaitem;
        UITableView tableView;
        string NAVIGATION_TEXT = AppTheme.ADNavigationText;
        static nfloat NAVIGATION_Title_Width = 230;
		static nfloat NAVIGATION_Title_Height = 30;
		static nfloat imgStar_Width = 30;
		static nfloat imgStar_Height = 30;
		static nfloat agendaImageLeftMargin = 35;
		static nfloat agendaImageTopMargin = 20;
		static nfloat agendaImageWidth = 300;
		static nfloat agendaImageHeight = 200;
		static nfloat agendaNameLeftMargin = 30;
		static nfloat agendaNameTopMargin = 20;
		static nfloat agendaNameRightMargin = 60;
		static nfloat locationLeftMargin = 30;
		static nfloat locationHeight = 20;
		static nfloat locationTopMargin = 5;
		static nfloat timingsLeftMargin = 30;
		static nfloat timingsHeight = 20;
		static nfloat timingsTopMargin = 3;
		static nfloat lineViewLeftPadding = 30;
		static nfloat lineViewHeight = 2;
		static nfloat lineViewTopMargin = 20;
		static nfloat descLeftmargin = 30;
		static nfloat descTopmargin = 20;


        public AgendaDetailController(BuiltAgendaItem builtAgendaItem)
        {
            this._builtAgendaitem = builtAgendaItem;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = AppTheme.ADBackgroundColor;
            NavigationItem.TitleView = new UILabel(new CGRect(0, 0, NAVIGATION_Title_Width, NAVIGATION_Title_Height))
            {
                Text = NAVIGATION_TEXT,
                TextAlignment = UITextAlignment.Left,
                Font = AppTheme.ADNavigationTitleFont,
                TextColor = AppTheme.ADNavigationTitle
            };
            UIButton imgStar = UIButton.FromType(UIButtonType.Custom);
            imgStar.SetImage(UIImage.FromBundle(AppTheme.ADStarImage), UIControlState.Normal);
            imgStar.Frame = new CGRect(0, 0, imgStar_Width, imgStar_Height);
            UIImageView imgShare = new UIImageView(UIImage.FromBundle(AppTheme.ADStarImage));
            NavigationItem.RightBarButtonItems = new[] 
            {
                new UIBarButtonItem(imgStar),
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) {Width = 10},
                new UIBarButtonItem(imgShare) 
            };
            tableView = new UITableView();
            tableView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;

        }
        private void SetHeader()
        {
            nfloat height = 0;

            UIView header = new UIView();
            var agendaImage = new UIImageView(UIImage.FromBundle(AppTheme.ADIconImage));
            agendaImage.Frame = new CGRect(agendaImageLeftMargin, agendaImageTopMargin, agendaImageWidth, agendaImageHeight);
            height += agendaImageTopMargin + agendaImage.Frame.Height + 5;
            var lineViewTop = new UIView(new CGRect(lineViewLeftPadding, agendaImage.Frame.Bottom + lineViewTopMargin, View.Frame.Width, lineViewHeight))
            {
                BackgroundColor = AppTheme.ADLineviewColor,
            };

            height += lineViewTopMargin + lineViewTop.Frame.Height;
            header.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            header.AutosizesSubviews = true;
            var agendaName = new UILabel(new CGRect(agendaImageLeftMargin, lineViewTop.Frame.Bottom + agendaNameTopMargin, View.Frame.Width - agendaNameRightMargin, 0))
            {
                Text = _builtAgendaitem.name,
                TextColor = AppTheme.ADNameTextColor,
				Font = AppFonts.ProximaNovaRegular(18),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
            };
            agendaName.SizeToFit();
            agendaName.Frame = new CGRect(agendaNameLeftMargin, lineViewTop.Frame.Bottom + agendaNameTopMargin, View.Frame.Width - agendaNameRightMargin, agendaName.Frame.Height);
            height += agendaNameTopMargin + agendaName.Frame.Height;
            var location = new UILabel(new CGRect(locationLeftMargin, agendaName.Frame.Bottom + locationTopMargin, View.Frame.Width - (locationLeftMargin * 2), locationHeight))
            {
                Text = _builtAgendaitem.location, //"Location",
                TextColor = AppTheme.ADLocationColor,
				Font = AppFonts.ProximaNovaRegular(14),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };
            height += locationTopMargin + locationHeight;

            var timings = new UILabel(new CGRect(timingsLeftMargin, location.Frame.Bottom + timingsTopMargin, View.Frame.Width - (timingsLeftMargin * 2), timingsHeight))
            {
                Text = _builtAgendaitem.start_time + "  " + _builtAgendaitem.end_time, //"Timings",
                TextColor = AppTheme.ADTimingColor,
				Font = AppFonts.ProximaNovaRegular(14),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };
            height += timingsTopMargin + timingsHeight;
            var lineView = new UIView(new CGRect(lineViewLeftPadding, timings.Frame.Bottom + lineViewTopMargin, View.Frame.Width, lineViewHeight))
            {
                BackgroundColor = AppTheme.ADLineviewColor,
            };
            height += lineViewTopMargin + lineViewHeight;
            var description = new UILabel(new CGRect(descLeftmargin, lineView.Frame.Bottom + descTopmargin, View.Frame.Width - (timingsLeftMargin * 2), 0))
            {
                Text = _builtAgendaitem.description, //"Timings",
                TextColor = AppTheme.ADTimingColor,
				Font = AppFonts.ProximaNovaRegular(14),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };
            var h = Helper.getTextHeight(description.Text, description.Frame.Width, UILineBreakMode.WordWrap, UITextAlignment.Left, description.Font, description);

            var rect = description.Frame;
            rect.Height = h;

            description.Frame = rect;
            height += descTopmargin + description.Frame.Height;

            header.Frame = new CGRect(0, 0, View.Frame.Width, height + 30);
            tableView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
            var headerDetail = new UIView();
            headerDetail.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            headerDetail.AutosizesSubviews = true;

            headerDetail.AddSubviews(agendaImage, lineViewTop, agendaName, location, timings, lineView, description);
            header.AddSubviews(headerDetail);
            View.AddSubview(header);
            View.AddSubview(tableView);

            tableView.TableHeaderView = new UIView(new CGRect(0, 0, View.Frame.Width, height + 30));
            SetFooter();
        }
        private void SetFooter()
        {
            UIView footer = new UIView(CGRect.Empty);
            tableView.TableFooterView = footer;
        }
        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (tableView.TableHeaderView == null)
                SetHeader();
        }
    }
}