using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace LoremasterHelper
{
    public class Config
    {
        public const int OutputCacheDuration = 600;
        public static string OAuthBaseUrl(string region)
        {
            //this is a hack to always use the eu authentication portal, since production cannot connect to the us auth portal
            return string.Format(ConfigurationManager.AppSettings["AuthApiRoot"], "eu");
        }

        public static string DataApiBaseUrl(string region)
        {
            return string.Format(ConfigurationManager.AppSettings["DataApiRoot"], region);
        }

        public static string CommunityApiBaseUrl(string region)
        {
            return string.Format(ConfigurationManager.AppSettings["ComApiRoot"], region);
        }

        private static string[] _supportedRegions;
        public static string[] SupportedRegions
        {
            get
            {
                if (_supportedRegions == null)
                {
                    var regionString = ConfigurationManager.AppSettings["SupportedRegions"];
                    _supportedRegions = regionString.Split(',');
                }
                return _supportedRegions;
            }
        }
        public static bool IsRegionSupported(string region)
        {
            if (string.IsNullOrWhiteSpace(region))
            {
                return false;
            }
            return SupportedRegions.Contains(region);
        }

        public static int ClientCrediantialExpiryPadding
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["ClientCrediantialExpiryPadding"]);
            }
        }

        public static string AuthApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthApiKey"];
            }
        }

        public static string AuthApiSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["AuthApiSecret"];
            }
        }

        public static string OAuthScope
        {
            get
            {
                return ConfigurationManager.AppSettings["OAuthScope"];
            }
        }

        public static string CanonicalUrlBase
        {
            get
            {
                return ConfigurationManager.AppSettings["CanonicalUrlBase"];
            }
        }

        public static string OAuthResponseType
        {
            get
            {
                return ConfigurationManager.AppSettings["OAuthResponseType"];
            }
        }

        public static string OAuthRedirectUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["OAuthRedirectUrl"];
            }
        }

        public static string WowVersionPhrase
        {
            get
            {
                return ConfigurationManager.AppSettings["WowVersionPhrase"];
            }
        }

       

        private static int? _simThreshold;
        public static int SimQuestThreshold
        {
            get
            {
                if (!_simThreshold.HasValue)
                {
                    _simThreshold = int.Parse(ConfigurationManager.AppSettings["SimQuestThreshold"]);
                }
                return _simThreshold.Value;
            }
        }

        private static int? _accCache;
        public static int AccountCharacterCacheSeconds
        {
            get
            {
                if (!_accCache.HasValue)
                {
                    _accCache = int.Parse(ConfigurationManager.AppSettings["AccountCharactersCacheSeconds"]);
                }
                return _accCache.Value;
            }
        }

        private static int? _detCache;
        public static int CharacterDetailsCacheSeconds
        {
            get
            {
                if (!_detCache.HasValue)
                {
                    _detCache = int.Parse(ConfigurationManager.AppSettings["CharacterDetailCacheSeconds"]);
                }
                return _detCache.Value;
            }
        }

    }
}