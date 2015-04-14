using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;

namespace ConferenceAppiOS
{
    public class REComposeSheetView : BaseViewController
    {
        UIView attachmentView;
        UIImageView attachmentImageView;
        UINavigationItem navigationItem;
        UINavigationBar navigationBar;
        UIView textViewContainer;
        UIView separatorView;
        public UILabel textCountLabel;
        UITextView textView;
		public static nfloat kMaxCharacterCount = 140;
        CGRect rectTmp;
        public Action<string> textData;
        BuiltTwitter currentModel;


        public REComposeSheetView(CGRect rect,BuiltTwitter currentModel ) 
        {
            this.currentModel = currentModel;
            rectTmp = rect;
            View.Frame = rect;
           
        }
     public override void ViewDidLoad()
{
 	 base.ViewDidLoad();

        this.View.Layer.CornerRadius = 10.0f;
        navigationBar = new UINavigationBar(new CGRect( 0,0,View.Frame.Size.Width,44)); 
        navigationBar.AutoresizingMask= UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleRightMargin;

        navigationItem =new UINavigationItem(); 
            navigationItem.Title="";
        navigationBar.Items= new[]{ navigationItem}; 
        
         if(navigationItem.RightBarButtonItem!=null)
         {
             navigationItem.RightBarButtonItem.Enabled = false;
         }
        
        
        UIBarButtonItem cancelButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Cancel,(s,e)=>
        {
            AppDelegate.instance().rootViewController.closeDialogue();
        }); 
       
        cancelButtonItem.TintColor =UIColor.FromRGBA(34.0f/255.0f,97.0f/255.0f,221.0f/255.0f,1f);
        
        UILabel label = new UILabel(CGRect.Empty); 
        label.BackgroundColor = UIColor.Clear;
        label.Font = UIFont.BoldSystemFontOfSize(18.0f);
        label.TextAlignment = UITextAlignment.Center;
        label.TextColor = UIColor.Black;
        this.navigationItem.TitleView= label;
        label.Text= "Reply";
        label.SizeToFit();

        UIBarButtonItem postButtonItem =new UIBarButtonItem(UIBarButtonSystemItem.Done,(s, e)=>
        {
            if (textData != null)
                textData(textView.Text);
            AppDelegate.instance().rootViewController.closeDialogue();
        }); 
        postButtonItem.TintColor =UIColor.FromRGBA(34.0f/255.0f,97.0f/255.0f,221.0f/255.0f,1f);
        
        if (!REUIKitIsFlatMode()) {
            navigationItem.SetLeftBarButtonItem(cancelButtonItem,true);
            navigationItem.SetRightBarButtonItem(postButtonItem, true);
            navigationItem.TitleView = label;
        } else {
            UIBarButtonItem leftSeperator = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace,null,null);
            leftSeperator.Width = 5.0f;
            UIBarButtonItem rightSeperator =  new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace,null,null);
            rightSeperator.Width = 5.0f;
            navigationItem.LeftBarButtonItems = new []{leftSeperator, cancelButtonItem};
            navigationItem.RightBarButtonItems  = new[] {rightSeperator, postButtonItem};
        }
        
        separatorView = new UIView(new CGRect(0f, 44f, View.Frame.Size.Width - (REUIKitIsFlatMode() ? 20f : 0f), 0.5f)); 
        separatorView.ClipsToBounds = true;
        separatorView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
        separatorView.BackgroundColor = UIColor.DarkGray;
        
            View.AddSubview(separatorView);

        textViewContainer = new UIView((new CGRect(0, 44, View.Frame.Size.Width - (REUIKitIsFlatMode() ? 20 : 0), View.Frame.Size.Height - 44)));
        textViewContainer.ClipsToBounds = true;
        textViewContainer.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;;
        textView = new UITextView(new CGRect(REUIKitIsFlatMode() ? 8 : 0, 0, View.Frame.Size.Width - 100, View.Frame.Size.Height - 47));
        textView.BackgroundColor = UIColor.White;
        textView.Font = UIFont.SystemFontOfSize( REUIKitIsFlatMode() ? 17 : 21);
        textView.ContentInset = new UIEdgeInsets(0, 0, 20, 0);
        textView.Text = "@" + currentModel.username;
        textView.Bounces = true;
        textView.WeakDelegate = this;
        textView.Delegate = new MyTextViewDelegate(this);
        textViewContainer.AddSubview(textView);
        
        
        textCountLabel = new UILabel(new CGRect(textViewContainer.Frame.Size.Width - 45, textViewContainer.Frame.Size.Height - 25, 50, 18));
        textCountLabel.TextColor = UIColor.FromRGBA(34.0f/255.0f,97.0f/255.0f,221.0f/255.0f,1f);
        textCountLabel.TextAlignment = UITextAlignment.Center;
        textCountLabel.Text = "140";
        textCountLabel.BackgroundColor = UIColor.Clear;
        
        textViewContainer.AddSubview(textCountLabel);
        attachmentView = new UIView(new CGRect(View.Frame.Size.Width - 84, 54, 84, 79));
        View.AddSubview(attachmentView);
        
        attachmentImageView = new UIImageView(new CGRect(REUIKitIsFlatMode() ? 2 : 6, 2, 72, 72));
        if (!REUIKitIsFlatMode())
        {
            attachmentImageView.Layer.CornerRadius = 3.0f;
        }
        attachmentImageView.Layer.MasksToBounds = true;
        attachmentView.AddSubview(attachmentImageView);
        
        var attachmentContainerView = new UIImageView();
            attachmentContainerView .Bounds=attachmentView.Bounds;
        if (!REUIKitIsFlatMode()) 
        {
           
        }
        attachmentView.AddSubview(attachmentContainerView);
        attachmentView.Hidden = true;
    
        navigationBar.Opaque = true;
        navigationBar.BackgroundColor=UIColor.FromWhiteAlpha(0.85f,1.0f);
        View.AddSubviews(navigationBar,textViewContainer);
    }

     public override void ViewWillLayoutSubviews()
     {
         base.ViewWillLayoutSubviews();
         navigationBar.Frame = new CGRect(0, 0, View.Frame.Size.Width, 44);
         separatorView.Frame = new CGRect(0f, 44f, View.Frame.Size.Width - (REUIKitIsFlatMode() ? 20f : 0f), 0.5f);
         textViewContainer.Frame = new CGRect(0f, 44f, View.Frame.Size.Width , View.Frame.Size.Height - 44f);
         textView.Frame = new CGRect(REUIKitIsFlatMode() ? 8 : 0, 0, View.Frame.Size.Width , View.Frame.Size.Height - 47);
         textCountLabel.Frame = new CGRect(textViewContainer.Frame.Size.Width - 45, textViewContainer.Frame.Size.Height - 25, 50, 18);
         attachmentView.Frame =new CGRect(View.Frame.Size.Width - 84, 54, 84, 79);
         attachmentImageView.Frame = new CGRect(REUIKitIsFlatMode() ? 2 : 6, 2, 72, 72);


     }

        private bool onceToken;
        private bool REUIKitIsFlatMode()
        {
            bool isUIKitFlatMode = false;
            if (onceToken == null)
            {
           
            if (UIApplication.SharedApplication.KeyWindow!=null) {
               
                isUIKitFlatMode = true;
            } else 
            {
               
                isUIKitFlatMode = true;
            }

            }
            return isUIKitFlatMode;
           
        }

        [Export("textViewDidChange:")]
        public void MyTextVeiwChanged(UITextView textview)
        {
//            Console.WriteLine(textView.Text);
            var characterCount = kMaxCharacterCount - textView.Text.Length;
            textCountLabel.Text = string.Format("{0}d", characterCount);
            if (characterCount < 0)
            {
                textCountLabel.TextColor = UIColor.Red;
            }
            else if (characterCount < 20)
            {
                textCountLabel.TextColor = UIColor.Orange;
            }
            else
            {
                textCountLabel.TextColor = UIColor.FromRGBA(34.0f / 255.0f, 97.0f / 255.0f, 221.0f / 255.0f, 1);
            }
        }

        void textViewDidChange(UITextView textView)
        {
            var characterCount = kMaxCharacterCount - textView.Text.Length;
            textCountLabel.Text = string.Format("{0}d", characterCount);
            if (characterCount < 0)
            {
                textCountLabel.TextColor = UIColor.Red;
            }
            else if (characterCount < 20)
            {
                textCountLabel.TextColor = UIColor.Orange;
            }
            else
            {
                textCountLabel.TextColor = UIColor.FromRGBA(34.0f / 255.0f, 97.0f / 255.0f, 221.0f / 255.0f, 1);
            }
        }

    }

}