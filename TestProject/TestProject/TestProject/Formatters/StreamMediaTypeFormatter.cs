using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using System.Web;

namespace TestProject.Formatters
{
    public class StreamMediaTypeFormatter : MediaTypeFormatter
    {
        public StreamMediaTypeFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
        }

        public override bool CanReadType(Type type)
        {
            bool canRead = typeof(StreamContent) == type;
            return canRead;
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }

        public override Task<object> ReadFromStreamAsync(Type type,
            Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            return Task.FromResult((object)readStream);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream,
                HttpContent content, IFormatterLogger formatterLogger, System.Threading.CancellationToken cancellationToken)
        {
            return ReadFromStreamAsync(type, readStream, content, formatterLogger);
        }

    }
}