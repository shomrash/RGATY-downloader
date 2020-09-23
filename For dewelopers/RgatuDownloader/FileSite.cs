using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RgatuDownloader
{
    public class FileSite
    {
        const string contLine = "content/0/";
        const string forcd = "?forcedownload=1";
        public FileSite()
        {

        }

        public FileSite(string _url)
        {
            url = new Uri(_url, UriKind.Absolute);

            name = _url.Remove(0, _url.IndexOf(contLine) + contLine.Length);
            name = name.Remove(name.IndexOf(forcd), name.Length - name.IndexOf(forcd));
            name = System.Uri.UnescapeDataString(name);
        }
        public Uri url;
        public string name;
    }
}
