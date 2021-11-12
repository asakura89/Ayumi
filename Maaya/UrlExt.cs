using System.Collections.Specialized;
using System.Web;

namespace Maaya;

public static class UrlExt {
    public static Boolean IsLocalUrl(this String url, params Uri[] allowedUris) {
        if (String.IsNullOrEmpty(url))
            return false;

        if (String.IsNullOrWhiteSpace(url))
            return false;

        Boolean localUrl = url.IsLocalUrl();
        if (localUrl)
            return true;

        Boolean httpUrl = url.StartsWith("http:", StringComparison.InvariantCultureIgnoreCase);
        Boolean httpsUrl = url.StartsWith("https:", StringComparison.InvariantCultureIgnoreCase);
        if (httpUrl || httpsUrl) {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return false;

            var absolute = new Uri(url);
            Boolean inAllowedUrls = allowedUris
                .Select(uri => uri.Host)
                .Any(host => absolute.Host.Equals(host, StringComparison.InvariantCultureIgnoreCase));

            return inAllowedUrls;
        }

        return false;
    }

    static Boolean IsLocalUrl(this String url) {
        if (url[0] == '/') {
            if (url.Length == 1) // "/" valid
                return true;

            if (url.Length > 1 && (url[1] == '/' || url[1] == '\\')) // "//" or "/\" invalid
                return false;

            return true;
        }

        if (url[0] == '~') {
            if (url.Length == 1) // "~" invalid
                return false;

            if (url.Length == 2 && url[1] == '/') // "~/" valid
                return true;

            if (url.Length > 2 && (url[2] == '/' || url[2] == '\\')) // "~//" or "~/\" invalid
                return false;

            return true;
        }

        return false;
    }

    // NOTE: Return value determine if need to processed further or not
    static Boolean ParseObject(Object source, out String result) {
        String cleaned;
        if (source == null) {
            result = null;
            return false;
        }

        cleaned = source as String;
        if (cleaned == null) {
            result = null;
            return false;
        }

        cleaned = cleaned.Trim();
        if (String.IsNullOrEmpty(cleaned)) {
            result = cleaned;
            return false;
        }

        result = cleaned;
        return true;
    }

    public static String AsCleanedString(this Object source) {
        Boolean okForFurtherProcessing = ParseObject(source, out String cleaned);
        return !okForFurtherProcessing ? cleaned : HttpUtility.HtmlEncode(cleaned);
    }

    public static String AsCleanedLink(this Object source) {
        Boolean okForFurtherProcessing = ParseObject(source, out String cleaned);
        if (!okForFurtherProcessing)
            return cleaned;

        Boolean localUrl = cleaned.IsLocalUrl();
        if (localUrl)
            return cleaned;

        var uriBuilder = new UriBuilder(cleaned);
        IList<String> cleanedSegments = new List<String>();
        foreach (String segment in uriBuilder.Uri.Segments) {
            String cleanedSegment = segment.Replace("/", String.Empty);
            if (cleanedSegment == String.Empty)
                continue;

            cleanedSegments.Add(HttpUtility.UrlEncode(HttpUtility.UrlDecode(cleanedSegment)));
            // NOTE: we're back and forth because we can't turn off the UriBuilder's default url encoder
            // and the UriBuilder's default url encoder did not escape singiequote and normal brackets
        }
        uriBuilder.Path = String.Join("/", cleanedSegments);

        NameValueCollection queryStrings = HttpUtility.ParseQueryString(uriBuilder.Query);
        NameValueCollection shadowQueryStrings = HttpUtility.ParseQueryString(uriBuilder.Query);
        foreach (String key in shadowQueryStrings)
            queryStrings[key] = HttpUtility.UrlEncode(HttpUtility.UrlDecode(shadowQueryStrings[key]));
        uriBuilder.Query = HttpUtility.UrlDecode(queryStrings.ToString());
        uriBuilder.Fragment = HttpUtility.UrlEncode(HttpUtility.UrlDecode(uriBuilder.Fragment).Replace("#", String.Empty));

        // NOTE: hack to remove port
        if (uriBuilder.Uri.IsDefaultPort)
            uriBuilder.Port = -1;

        return uriBuilder.Uri.AbsoluteUri;
    }
}