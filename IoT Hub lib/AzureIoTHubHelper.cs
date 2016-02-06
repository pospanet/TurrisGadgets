using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pospa.NET.IoTHub
{
    public class AzureIoTHubHelper
    {
        #region Constants

        private const string IoTHubUrlPostfix = ".azure-devices.net";
        private const string ApiVersionQueryString = "2016-02-03";

        private const int DefaultHttpTimeout = 100;
        private const string MediaTypeForDeviceManagementApis = "application/json";

        private const string DeviceManagementRequestUriFormat = "/devices/{0}?api-version={1}";
        private const string DeviceMessageRequestUriFromat = "/devices/{0}/messages/events?api-version={1}";
        private const string CloudMessageRequestUriFormat = "/devices/{0}/messages/devicebound?api-version={1}";

        private const string CompleteCloudMessageRequestUriFormat =
            "/devices/{0}/messages/devicebound/{2}?api-version={1}";

        private const string RejectCloudMessageRequestUriFormat =
            "/devices/{0}/messages/devicebound/{2}?reject&api-version={1}";

        private const string AbandonCloudMessageRequestUriFormat =
            "/devices/{0}/messages/devicebound/{2}?abandon&api-version={1}";

        private const string GetDeviceListRequestUriFormat = "/devices?top={0}&api-version={1}";

        private const string MessagePropertyHeaderPrefix = "IoTHub-app-";
        private const string MessageCorrelationIdHeaderKey = "IoTHub-CorrelationId";
        private const string MessageMessageIdHeaderKey = "IoTHub-MessageId";
        private const string MessageUserIdHeaderKey = "IoTHub-UserId";
        private const string MessageLockTimeoutHeaderKey = "IoTHub-MessageLockTimeout";
        private const string MessageSequenceNumberHeaderKey = "IoTHub-SequenceNumber";
        private const string MessageEnquedTimeHeaderKey = "IoTHub-EnqueuedTime";
        private const string MessageExpiryTimeHeaderKey = "IoTHub-Expiry";
        private const string MessageAckHeaderKey = "IoTHub-Ack";
        private const string MessageToHeaderKey = "IoTHub-To";
        private const string MessageContentTypeHeaderKey = "Content-Type";
        private const string MessageContentEncodingHeaderKey = "Content-Encoding";

        private const string DeviceStatisticRequestUriFormat = "/statistics/devices?api-version={0}";
        private const string ServiceStatisticRequestUriFormat = "/statistics/service?api-version={0}";

        #endregion Constants

        #region Public methods

        #region Device identities

        public static async Task<Device> AddDeviceAsync(Device device, string iotHub, string sas)
        {
            return await AddDeviceAsync(device, iotHub, sas, CancellationToken.None);
        }

        public static async Task<Device> AddDeviceAsync(Device device, string iotHub, string sas,
            CancellationToken cancellationToken)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (string.IsNullOrWhiteSpace(device.Id))
            {
                throw new ArgumentException("The id of the device was not set.");
            }

            if (!string.IsNullOrEmpty(device.ETag))
            {
                throw new ArgumentException("The ETag should not be set while registering the device.");
            }

            if (device.Authentication == null)
            {
                device.Authentication = new AuthenticationMechanism();
            }


            Uri baseAddress = new UriBuilder("https", string.Concat(iotHub, IoTHubUrlPostfix), 443).Uri;
            Uri endPoint =
                new Uri(
                    string.Format(CultureInfo.InvariantCulture, DeviceManagementRequestUriFormat, device.Id,
                        ApiVersionQueryString),
                    UriKind.Relative);

            return await SendWebRequest<Device>(baseAddress, endPoint, HttpMethod.Put, sas, device, cancellationToken);
        }

        public static async Task<Device> GetDeviceAsync(string deviceId, string iotHub, string sas)
        {
            return await GetDeviceAsync(deviceId, iotHub, sas, CancellationToken.None);
        }

        public static async Task<Device> GetDeviceAsync(string deviceId, string iotHub, string sas,
            CancellationToken cancellationToken)
        {
            Uri baseAddress = new UriBuilder("https", string.Concat(iotHub, IoTHubUrlPostfix), 443).Uri;
            Uri endPoint =
                new Uri(
                    string.Format(CultureInfo.InvariantCulture, DeviceManagementRequestUriFormat, deviceId,
                        ApiVersionQueryString),
                    UriKind.Relative);
            return await SendWebRequest<Device>(baseAddress, endPoint, HttpMethod.Get, sas, null, cancellationToken);
        }

        public static async Task<Device> UpdateDeviceAsync(Device device, string iotHub, string sas)
        {
            return await UpdateDeviceAsync(device, iotHub, sas, CancellationToken.None);
        }

        public static async Task<Device> UpdateDeviceAsync(Device device, string iotHub, string sas,
            CancellationToken cancellationToken)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (string.IsNullOrWhiteSpace(device.Id))
            {
                throw new ArgumentException("The id of the device was not set.");
            }

            if (string.IsNullOrEmpty(device.ETag))
            {
                throw new ArgumentException("The ETag has to be set while updating the device.");
            }
            Uri baseAddress = new UriBuilder("https", string.Concat(iotHub, IoTHubUrlPostfix), 443).Uri;
            Uri endPoint =
                new Uri(
                    string.Format(CultureInfo.InvariantCulture, DeviceManagementRequestUriFormat, device.Id,
                        ApiVersionQueryString),
                    UriKind.Relative);

            return await SendWebRequest<Device>(baseAddress, endPoint, HttpMethod.Put, sas, device, cancellationToken);
        }

        public static async Task DeleteDeviceAsync(string deviceId, string iotHub, string sas)
        {
            await DeleteDeviceAsync(deviceId, iotHub, sas, CancellationToken.None);
        }

        public static async Task DeleteDeviceAsync(string deviceId, string iotHub, string sas,
            CancellationToken cancellationToken)
        {
            Uri baseAddress = new UriBuilder("https", string.Concat(iotHub, IoTHubUrlPostfix), 443).Uri;
            Uri endPoint =
                new Uri(
                    string.Format(CultureInfo.InvariantCulture, DeviceManagementRequestUriFormat, deviceId,
                        ApiVersionQueryString),
                    UriKind.Relative);
            await SendWebRequest<Device>(baseAddress, endPoint, HttpMethod.Delete, sas, null, cancellationToken);
        }

        public static async Task<IEnumerable<Device>> GetDeviceListAsync(string iotHub, string sas,
            int maxNumberOfDevices)
        {
            return await GetDeviceListAsync(iotHub, sas, maxNumberOfDevices, CancellationToken.None);
        }

        public static async Task<IEnumerable<Device>> GetDeviceListAsync(string iotHub, string sas,
            int maxNumberOfDevices, CancellationToken cancellationToken)
        {
            Uri baseAddress = new UriBuilder("https", string.Concat(iotHub, IoTHubUrlPostfix), 443).Uri;
            Uri endPoint =
                new Uri(
                    string.Format(CultureInfo.InvariantCulture, GetDeviceListRequestUriFormat, maxNumberOfDevices,
                        ApiVersionQueryString),
                    UriKind.Relative);
            return await SendWebRequest<Device[]>(baseAddress, endPoint, HttpMethod.Get, sas, null, cancellationToken);
        }

        #endregion Device identities

        #region Statistic

        public static async Task<DeviceIdentitiesStatistics> GetDeviceStatisticAsync(string iotHub, string sas)
        {
            return await GetDeviceStatisticAsync(iotHub, sas, CancellationToken.None);
        }

        public static async Task<DeviceIdentitiesStatistics> GetDeviceStatisticAsync(string iotHub, string sas,
            CancellationToken cancellationToken)
        {
            Uri baseAddress = new UriBuilder("https", string.Concat(iotHub, IoTHubUrlPostfix), 443).Uri;
            Uri endPoint =
                new Uri(
                    string.Format(CultureInfo.InvariantCulture, DeviceStatisticRequestUriFormat, ApiVersionQueryString),
                    UriKind.Relative);
            return
                await
                    SendWebRequest<DeviceIdentitiesStatistics>(baseAddress, endPoint, HttpMethod.Get, sas, null,
                        cancellationToken);
        }

        public static async Task<ServiceStatistics> GetServiceStatisticAsync(string iotHub, string sas)
        {
            return await GetServiceStatisticAsync(iotHub, sas, CancellationToken.None);
        }

        public static async Task<ServiceStatistics> GetServiceStatisticAsync(string iotHub, string sas,
            CancellationToken cancellationToken)
        {
            Uri baseAddress = new UriBuilder("https", string.Concat(iotHub, IoTHubUrlPostfix), 443).Uri;
            Uri endPoint =
                new Uri(
                    string.Format(CultureInfo.InvariantCulture, ServiceStatisticRequestUriFormat, ApiVersionQueryString),
                    UriKind.Relative);
            return
                await
                    SendWebRequest<ServiceStatistics>(baseAddress, endPoint, HttpMethod.Get, sas, null,
                        cancellationToken);
        }

        #endregion Statistic

        #region Device Messaging

        public static async Task SendMessageDataAsync(string deviceId, string iotHub, string sas, DeviceMessage message)
        {
            await SendMessageDataAsync(deviceId, iotHub, sas, message, CancellationToken.None);
        }

        public static async Task SendMessageDataAsync(string deviceId, string iotHub, string sas, DeviceMessage message,
            CancellationToken cancellationToken)
        {
            Uri baseAddress = new UriBuilder("https", string.Concat(iotHub, IoTHubUrlPostfix), 443).Uri;
            Uri endPoint =
                new Uri(
                    string.Format(CultureInfo.InvariantCulture, DeviceMessageRequestUriFromat, deviceId,
                        ApiVersionQueryString),
                    UriKind.Relative);
            IDictionary<string, string> properties = new Dictionary<string, string>();
            if (message.CorrelationId != string.Empty)
            {
                properties.Add(MessageCorrelationIdHeaderKey, message.CorrelationId);
            }
            if (message.MessageId != string.Empty)
            {
                properties.Add(MessageMessageIdHeaderKey, message.MessageId);
            }
            if (message.UserId != string.Empty)
            {
                properties.Add(MessageUserIdHeaderKey, message.UserId);
            }
            foreach (KeyValuePair<string, string> pair in message.Properties)
            {
                properties.Add(string.Concat(MessagePropertyHeaderPrefix, pair.Key), pair.Value);
            }
            await
                SendWebRequest(baseAddress, endPoint, HttpMethod.Post, sas, message.Data, cancellationToken, properties);
        }

        public static async Task<CloudMessage> ReceiveMessageDataAsync(string deviceId, string iotHub, string sas,
            int lockTimeout)
        {
            return await ReceiveMessageDataAsync(deviceId, iotHub, sas, lockTimeout, CancellationToken.None);
        }

        public static async Task<CloudMessage> ReceiveMessageDataAsync(string deviceId, string iotHub, string sas,
            int lockTimeout,
            CancellationToken cancellationToken)
        {
            Uri baseAddress = new UriBuilder("https", string.Concat(iotHub, IoTHubUrlPostfix), 443).Uri;
            Uri endPoint =
                new Uri(
                    string.Format(CultureInfo.InvariantCulture, CloudMessageRequestUriFormat, deviceId,
                        ApiVersionQueryString),
                    UriKind.Relative);
            IDictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add(MessageLockTimeoutHeaderKey, lockTimeout.ToString());
            HttpResponseMessage responseMessage =
                await SendWebRequest(baseAddress, endPoint, HttpMethod.Get, sas, null, cancellationToken, properties);
            CloudMessage cloudMessage = new CloudMessage();
            foreach (
                KeyValuePair<string, IEnumerable<string>> pair in
                    responseMessage.Headers.Where(h => h.Key.StartsWith(MessagePropertyHeaderPrefix)))
            {
                cloudMessage.Properties.Add(pair.Key.Substring(MessagePropertyHeaderPrefix.Length),
                    pair.Value.FirstOrDefault());
            }
            cloudMessage.Etag = responseMessage.Headers.ETag.Tag;
            cloudMessage.SequenceNumber =
                Convert.ToInt64(responseMessage.Headers.GetValues(MessageSequenceNumberHeaderKey).FirstOrDefault());
            cloudMessage.EnqueuedTime =
                Convert.ToDateTime(responseMessage.Headers.GetValues(MessageEnquedTimeHeaderKey).FirstOrDefault());
            IEnumerable<string> values;
            if (responseMessage.Headers.TryGetValues(MessageExpiryTimeHeaderKey, out values) &&
                !string.IsNullOrEmpty(values.FirstOrDefault()))
            {
                cloudMessage.ExpiryTime = Convert.ToDateTime(values.FirstOrDefault());
            }
            cloudMessage.Ack = responseMessage.Headers.GetValues(MessageAckHeaderKey).FirstOrDefault();
            cloudMessage.MessageId = responseMessage.Headers.GetValues(MessageMessageIdHeaderKey).FirstOrDefault();
            cloudMessage.CorrelationId =
                responseMessage.Headers.GetValues(MessageCorrelationIdHeaderKey).FirstOrDefault();
            if (responseMessage.Headers.TryGetValues(MessageUserIdHeaderKey, out values) &&
                !string.IsNullOrEmpty(values.FirstOrDefault()))
            {
                cloudMessage.UserId = values.FirstOrDefault();
            }
            cloudMessage.To = responseMessage.Headers.GetValues(MessageToHeaderKey).FirstOrDefault();
            if (responseMessage.Headers.TryGetValues(MessageContentTypeHeaderKey, out values) &&
                !string.IsNullOrEmpty(values.FirstOrDefault()))
            {
                cloudMessage.DataType = values.FirstOrDefault();
            }
            if (responseMessage.Headers.TryGetValues(MessageContentEncodingHeaderKey, out values) &&
                !string.IsNullOrEmpty(values.FirstOrDefault()))
            {
                cloudMessage.DataEncoding = values.FirstOrDefault();
            }
            string content = await responseMessage.Content.ReadAsStringAsync();
            return cloudMessage;
        }

        public static async Task CompleteMessage(string etag, string iotHub, string sas, string sequenceNumber)
        {
            await CompleteMessage(etag, iotHub, sas, sequenceNumber, CancellationToken.None);
        }

        public static async Task CompleteMessage(string etag, string iotHub, string sas, string sequenceNumber,
            CancellationToken cancellationToken)
        {
            Uri baseAddress = new UriBuilder("https", string.Concat(iotHub, IoTHubUrlPostfix), 443).Uri;
            Uri endPoint =
                new Uri(
                    string.Format(CultureInfo.InvariantCulture, CompleteCloudMessageRequestUriFormat, etag,
                        ApiVersionQueryString, sequenceNumber),
                    UriKind.Relative);
            await
                SendWebRequest(baseAddress, endPoint, HttpMethod.Delete, sas, null, cancellationToken);
        }

        public static async Task RejectMessage(string etag, string iotHub, string sas, string sequenceNumber)
        {
            await RejectMessage(etag, iotHub, sas, sequenceNumber, CancellationToken.None);
        }

        public static async Task RejectMessage(string etag, string iotHub, string sas, string sequenceNumber,
            CancellationToken cancellationToken)
        {
            Uri baseAddress = new UriBuilder("https", string.Concat(iotHub, IoTHubUrlPostfix), 443).Uri;
            Uri endPoint =
                new Uri(
                    string.Format(CultureInfo.InvariantCulture, RejectCloudMessageRequestUriFormat, etag,
                        ApiVersionQueryString, sequenceNumber),
                    UriKind.Relative);
            await
                SendWebRequest(baseAddress, endPoint, HttpMethod.Delete, sas, null, cancellationToken);
        }

        public static async Task AbandonMessage(string etag, string iotHub, string sas, string sequenceNumber)
        {
            await AbandonMessage(etag, iotHub, sas, sequenceNumber, CancellationToken.None);
        }

        public static async Task AbandonMessage(string etag, string iotHub, string sas, string sequenceNumber,
            CancellationToken cancellationToken)
        {
            Uri baseAddress = new UriBuilder("https", string.Concat(iotHub, IoTHubUrlPostfix), 443).Uri;
            Uri endPoint =
                new Uri(
                    string.Format(CultureInfo.InvariantCulture, AbandonCloudMessageRequestUriFormat, etag,
                        ApiVersionQueryString, sequenceNumber),
                    UriKind.Relative);
            await
                SendWebRequest(baseAddress, endPoint, HttpMethod.Post, sas, null, cancellationToken);
        }

        #endregion Device Messaging

        #endregion Public methods

        #region Private methods

        private static async Task<HttpResponseMessage> SendWebRequest(Uri baseAddress, Uri endPoint,
            HttpMethod httpMethod, string sas, object content, CancellationToken cancellationToken,
            IDictionary<string, string> properties = null)
        {
            using (HttpRequestMessage message = new HttpRequestMessage(httpMethod, endPoint))
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = baseAddress;
                httpClient.Timeout = TimeSpan.FromSeconds(DefaultHttpTimeout);
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(MediaTypeForDeviceManagementApis));
                httpClient.DefaultRequestHeaders.ExpectContinue = false;
                message.Headers.Add(HttpRequestHeader.Authorization.ToString(), sas);
                if (content != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8,
                        MediaTypeForDeviceManagementApis);
                }
                message.Headers.ConnectionClose = true;

                if (properties != null)
                {
                    foreach (KeyValuePair<string, string> pair in properties)
                    {
                        message.Headers.Add(pair.Key, pair.Value);
                    }
                }

                HttpResponseMessage responseMessage = await httpClient.SendAsync(message, cancellationToken);
                if (responseMessage == null)
                {
                    throw new InvalidOperationException(string.Format(
                        "The response message was null when executing operation {0}.", httpMethod));
                }

                if (responseMessage.IsSuccessStatusCode)
                {
                    return responseMessage;
                }
                return null;
            }
        }

        private static async Task<T> SendWebRequest<T>(Uri baseAddress, Uri endPoint, HttpMethod httpMethod, string sas,
            object content, CancellationToken cancellationToken, IDictionary<string, string> properties = null)
        {
            HttpResponseMessage responseMessage =
                await SendWebRequest(baseAddress, endPoint, httpMethod, sas, content, cancellationToken, properties);
            if (responseMessage != null)
            {
                string serializedDevice = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(serializedDevice);
            }
            return default(T);
        }

        #endregion Private methods
    }
}