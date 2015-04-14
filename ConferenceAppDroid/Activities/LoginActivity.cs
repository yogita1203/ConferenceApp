using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Text;
using CommonLayer;
using ConferenceAppDroid.Utilities;
using Android.Views.InputMethods;
using ConferenceAppDroid.BroadcastReceivers;


namespace ConferenceAppDroid
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        private TextView termsAndConditionTextView, skipLoginTextView;
        RelativeLayout loadingContainer;
        Button loginButton;
        EditText userNameEditText, passwordEditText;
        ImageView clear_password_imageview, clear_username_imageview;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            OverridePendingTransition(Resource.Animation.up_from_bottom, Resource.Animation.hold);

            if (ActionBar != null)
            {
                ActionBar.Hide();
            }
            SetContentView(Resource.Layout.activity_login);

            init();
            String link = "By logging in, you are accepting the terms of the " + "<a href='com.builtio.vmworld.event://vmwareapp/TermsAndCondition'> <b>" + "License Agreement" + "</b> </a>" + "";
            termsAndConditionTextView.Text = Html.FromHtml(link).ToString();
            loadingContainer.Visibility = ViewStates.Gone;
            loginButton.Click += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(userNameEditText.Text))
                    {
                        Toast.MakeText(this, "Please enter username.", ToastLength.Short).Show();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(passwordEditText.Text))
                    {
                        Toast.MakeText(this, "Please enter password.", ToastLength.Short).Show();
                        return;
                    }
                    login();
                };

            userNameEditText.TextChanged += (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Text.ToString()) && e.Text.ToString().Length > 0)
                    {
                        clear_username_imageview.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        clear_username_imageview.Visibility = ViewStates.Gone;
                    }
                };

            clear_username_imageview.Click += (s, e) =>
                {
                    userNameEditText.Text = string.Empty;
                    clear_username_imageview.Visibility = ViewStates.Gone;
                };

            passwordEditText.TextChanged += (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Text.ToString()) && e.Text.ToString().Length > 0)
                    {
                        clear_password_imageview.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        clear_password_imageview.Visibility = ViewStates.Gone;
                    }
                };
            clear_password_imageview.Click += (s, e) =>
                {
                    passwordEditText.Text = string.Empty;
                    clear_password_imageview.Visibility = ViewStates.Gone;
                };
            skipLoginTextView.Click += (s, e) =>
                {
                    base.OnBackPressed();
                    OverridePendingTransition(Resource.Animation.up_from_bottom, Resource.Animation.hold);

                };
            // Create your application here
        }

        private void login()
        {
            try
            {
                
                loadingContainer.Visibility = ViewStates.Visible;
                setViewDisEnabled();
                DataManager.LoginExtension(userNameEditText.Text, passwordEditText.Text, DBHelper.Instance.Connection, (res) =>
                  {

                      if (res == null)
                      {
                          AppSettings.Instance.ApplicationUser = DataManager.GetCurrentUser(DBHelper.Instance.Connection).Result.application_user;
                          DataManager.SetCurrentUser(AppSettings.Instance.ApplicationUser);
                          DataManager.fetchAfterLoginAsync(DBHelper.Instance.Connection, AppSettings.Instance.ApplicationUser, (result, surveyCount) =>
                            {
                                AppSettings.Instance.NewSurveyCount = surveyCount;
                                RunOnUiThread(() =>
                                  {
                                      userNameEditText.Text = "";
                                      passwordEditText.Text = "";
                                      loadingContainer.Visibility = ViewStates.Gone;
                                      Intent intent = new Intent(LoginReceivers.action);
                                      SendBroadcast(intent);
                                      Finish();
                                  });
                            });
                      }
                      else
                      {
                          passwordEditText.Text = string.Empty;
                          try
                          {
                              string error_message = Helper.GetErrorMessage(res);
                              loadingContainer.Visibility = ViewStates.Gone;
                              Toast.MakeText(this, error_message, ToastLength.Short).Show();
                          }
                          catch
                          {
                              loadingContainer.Visibility = ViewStates.Gone;
                              Toast.MakeText(this, "Error occured", ToastLength.Short).Show();
                          }
                      }
                  });
            }
            catch
            { }
        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.up_from_bottom, Resource.Animation.hold);
        }

        public void hideSoftInputFromWindow()
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(passwordEditText.WindowToken, 0);
            imm.HideSoftInputFromWindow(userNameEditText.WindowToken, 0);
        }

        public void setViewDisEnabled()
        {
            hideSoftInputFromWindow();
        }

        private void init()
        {
            termsAndConditionTextView = FindViewById<TextView>(Resource.Id.termsAndConditionTextView);
            loadingContainer = FindViewById<RelativeLayout>(Resource.Id.session_loading_container);
            loginButton = FindViewById<Button>(Resource.Id.loginButton);
            userNameEditText = FindViewById<EditText>(Resource.Id.userNameEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText);
            clear_password_imageview = FindViewById<ImageView>(Resource.Id.clear_password_imageview);
            clear_username_imageview = FindViewById<ImageView>(Resource.Id.clear_username_imageview);
            skipLoginTextView = FindViewById<TextView>(Resource.Id.skipLoginTextView);
        }

        protected override void OnResume()
        {
            base.OnResume();
            new AppUtilities().isFromPauseResume(this, false);
        }
        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.hold, Resource.Animation.up_from_bottom_rev);
            new AppUtilities().isFromPauseResume(this, true);
        }
    }
}