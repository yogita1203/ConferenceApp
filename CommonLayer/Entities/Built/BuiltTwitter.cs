using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltTwitter
    {
        public string updated_at { get; set; }
        public string created_at { get; set; }
        public string username { get; set; }
        public string social_uid { get; set; }
        public string content_text { get; set; }
        public string social_type { get; set; }

        public string social_object { get; set; }
        Dictionary<string, object> result = new Dictionary<string, object>();
        public Dictionary<string, object> social_object_dict
        {
            get
            {
                if (social_object != null)
                {
                    try
                    {
                        result = JsonConvert.DeserializeObject<Dictionary<string, object>>(social_object);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                return result;
            }
            set
            {
                result = value;
            }

        }

        public Dictionary<string, object> user_dict
        {
            get
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                if (social_object_dict != null)
                {
                    try
                    {
                        var firstResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(social_object);
                        result = JsonConvert.DeserializeObject<Dictionary<string, object>>(firstResult["user"].ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                }
                return result;
            }
        }

        public User user
        {
            get
            {
                User result = null;
                if (social_object_dict != null)
                {
                    try
                    {
                        var firstResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(social_object);
                        result = JsonConvert.DeserializeObject<User>(firstResult["user"].ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                }
                return result;
            }
        }

        public Entities entities
        {
            get
            {
                Entities result = null;
                if (social_object_dict != null)
                {
                    try
                    {
                        var firstResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(social_object);
                        result = JsonConvert.DeserializeObject<Entities>(firstResult["entities"].ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                }
                return result;
            }
        }

        public string posted_on { get; set; }
        public List<object> tags { get; set; }
        public object app_user_object_uid { get; set; }
        public bool published { get; set; }
        public string uid { get; set; }
        public int _version { get; set; }

        public bool favourite
        {
            get
            {
                bool result = false;
                if (social_object != null)
                {
                    try
                    {
                        var firstResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(social_object);
                        result = JsonConvert.DeserializeObject<bool>(firstResult["favorited"].ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                return result;
            }
        }

        public bool retweeted
        {
            get
            {
                bool result = false;
                if (social_object != null)
                {
                    try
                    {
                        var firstResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(social_object);
                        result = JsonConvert.DeserializeObject<bool>(firstResult["retweeted"].ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                return result;
            }
        }

        public Dictionary<string, object> instagramDict
        {
            get
            {
                Dictionary<String, Object> result = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(this.social_type) && this.social_type.ToLower() == "instagram")
                {
                    try
                    {
                        result = JsonConvert.DeserializeObject<Dictionary<string, object>>(social_object);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                return result;
            }
        }

        public string tweetOwnerUserID
        {
            get
            {
                string result = string.Empty;
                if (this.user != null)
                {
                    try
                    {
                        result = this.user.id.ToString();
                    }
                    catch
                    { }
                }
                return result;
            }
        }


        public AllSocialModel getALLSocialModel()
        {
            AllSocialModel item = null;
            string profileImg = string.Empty;
            string comment = string.Empty;
            string likes = string.Empty;
            Dictionary<string, object> socialUser = new Dictionary<string, object>();
            Dictionary<string, object> socialComment = new Dictionary<string, object>();
            Dictionary<string,object> socialLikes = new Dictionary<string,object>();
            Dictionary<string,object> images = new Dictionary<string,object>();
            Dictionary<string, object> lowResoultion = new Dictionary<string, object>();
            string localType=string.Empty;
            string content = string.Empty;
            string url = string.Empty;
            string height = string.Empty;
            string width = string.Empty;

            if (this.social_object_dict != null)
            {
                try
                {
                    localType = social_object_dict["type"].ToString();
                    socialUser = JsonConvert.DeserializeObject<Dictionary<string, object>>(social_object_dict["user"].ToString());
                    socialComment = JsonConvert.DeserializeObject<Dictionary<string, object>>(social_object_dict["comments"].ToString());
                    socialLikes = JsonConvert.DeserializeObject<Dictionary<string, object>>(social_object_dict["likes"].ToString());
                    images= JsonConvert.DeserializeObject<Dictionary<string, object>>(social_object_dict["images"].ToString());
                    if(images!=null)
                    {
                        lowResoultion = JsonConvert.DeserializeObject<Dictionary<string, object>>(images["low_resolution"].ToString());
                        
                    }
                    
                }
                catch { }
            }

            if (this.content_text !=null)
            {
                content = content_text;
            }


            if (socialUser != null)
            {
                profileImg = socialUser["profile_picture"].ToString();
            }

            if (socialComment["count"] != null)
            {
                comment = socialComment["count"].ToString();
                
            }
            else
            {
                comment = "0";
            }


                if (socialLikes["count"] != null)
                {
                    likes = socialLikes["count"].ToString();
                }
                else
                {
                    likes = "0";
                }

                if (localType.ToLower() == "image")
                {
                    url = lowResoultion["url"].ToString();

                    width=lowResoultion["width"].ToString();
                    height=lowResoultion["height"].ToString();
                }


            if (social_type.ToLower() == "instagram" || social_type.ToLower() == "facebook" || social_type.ToLower() == "youtube")
            {
                item = new AllSocialModel
                {
                    username = this.username,
                    uid = this.uid,
                    user_image_url = profileImg,
                    commentsCount = comment,
                    likesCount=likes,
                    content_text = content,
                    type = localType,
                    media_url=url,
                    media_height = height,
                    media_width=width
                };

            }
            return item;
        }


    }
    public class Entities
    {
        public List<Hashtag> hashtags { get; set; }
        public List<object> symbols { get; set; }
        public List<Url3> urls { get; set; }
        public List<UserMention> user_mentions { get; set; }
        public List<Media> media { get; set; }
    }
    public class Media
    {
        public long id { get; set; }
        public string id_str { get; set; }
        public List<Int64> indices { get; set; }
        public string media_url { get; set; }
        public string media_url_https { get; set; }
        public string url { get; set; }
        public string display_url { get; set; }
        public string expanded_url { get; set; }
        public string type { get; set; }
        //public Sizes sizes { get; set; }
    }
    public class Hashtag
    {
        public string text { get; set; }
        public List<int> indices { get; set; }
    }
    public class UserMention
    {
        public string screen_name { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string id_str { get; set; }
        public List<int> indices { get; set; }
    }
    public class Url3
    {
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
        public List<int> indices { get; set; }

        public bool hasVideo
        {
            get
            {
                if (!string.IsNullOrEmpty(this.expanded_url))
                {
                    if (this.expanded_url.StartsWith("http://vines.s3.amazonaws.com/videos/") || this.expanded_url.StartsWith("http://youtu.be/") || this.expanded_url.StartsWith("https://youtu.be/"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                else
                {
                    return false;
                }
            }

        }

    }

    public class User
    {
        public Int64 id { get; set; }
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public string location { get; set; }
        public object profile_location { get; set; }
        public string url { get; set; }
        public UserEntities entities { get; set; }
        public bool @protected { get; set; }
        public Int64 followers_count { get; set; }
        public Int64 friends_count { get; set; }
        public Int64 listed_count { get; set; }
        public string created_at { get; set; }
        public Int64 favourites_count { get; set; }
        public Int64 utc_offset { get; set; }
        public string time_zone { get; set; }
        public bool geo_enabled { get; set; }
        public bool verified { get; set; }
        public Int64 statuses_count { get; set; }
        public string lang { get; set; }
        public bool contributors_enabled { get; set; }
        public bool is_translator { get; set; }
        public bool is_translation_enabled { get; set; }
        public string profile_background_color { get; set; }
        public string profile_background_image_url { get; set; }
        public string profile_background_image_url_https { get; set; }
        public bool profile_background_tile { get; set; }
        public string profile_image_url { get; set; }
        public string profile_image_url_https { get; set; }
        public string profile_banner_url { get; set; }
        public string profile_link_color { get; set; }
        public string profile_sidebar_border_color { get; set; }
        public string profile_sidebar_fill_color { get; set; }
        public string profile_text_color { get; set; }
        public bool profile_use_background_image { get; set; }
        public bool default_profile { get; set; }
        public bool default_profile_image { get; set; }
        public bool following { get; set; }
        public bool follow_request_sent { get; set; }
        public bool notifications { get; set; }
    }

    public class UserEntities
    {
        public Url url { get; set; }
        public Description description { get; set; }
    }

    public class Description
    {
        public List<object> urls { get; set; }
    }
    public class Url
    {
        public List<Url2> urls { get; set; }
    }
    public class Url2
    {
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
        public List<Int64> indices { get; set; }
    }

}
