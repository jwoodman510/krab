using Newtonsoft.Json;

namespace Krab.Api.ValueObjects.User
{
    public class RedditUser
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("over_18")]
        public bool IsOverEighteen { get; set; }
    }
}

/*
{
    "is_employee": false,
    "name": "stagnant_waffle",
    "created": 1333932449,
    "hide_from_robots": false,
    "is_suspended": false,
    "created_utc": 1333903649,
    "link_karma": 1,
    "in_beta": false,
    "comment_karma": 1201,
    "features": {
    "inline_image_previews_logged_out": true,
    "register_email_flow_test": {
        "variant": "control_1",
        "experiment_id": 23
    },
    "live_happening_now": true,
    "adserver_reporting": true,
    "hidden_thumbnails": {
        "variant": "control_2",
        "experiment_id": 21
    },
    "legacy_search_pref": true,
    "mobile_web_targeting": true,
    "adzerk_do_not_track": true,
    "sticky_comments": true,
    "upgrade_cookies": true,
    "ads_auto_refund": true,
    "ads_auction": true,
    "imgur_gif_conversion": true,
    "expando_events": true,
    "eu_cookie_policy": true,
    "force_https": true,
    "mobile_native_banner": true,
    "do_not_track": true,
    "new_loggedin_cache_policy": true,
    "registration_captcha": {
        "variant": "control_1",
        "experiment_id": 5
    },
    "https_redirect": true,
    "screenview_events": true,
    "pause_ads": true,
    "give_hsts_grants": true,
    "new_report_dialog": true,
    "subreddit_rules": true,
    "inline_image_previews_logged_in": true,
    "timeouts": true,
    "mobile_settings": true,
    "youtube_scraper": true,
    "activity_service_write": true,
    "ads_auto_extend": true,
    "post_embed": true,
    "autoexpand_media_subreddit_setting": true,
    "activity_service_read": true
    },
    "over_18": true,
    "is_gold": false,
    "is_mod": false,
    "id": "7e61f",
    "gold_expiration": null,
    "inbox_count": 0,
    "has_verified_email": null,
    "gold_creddits": 0,
    "suspension_expiration_utc": null
}
*/
