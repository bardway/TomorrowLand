﻿// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using TLAuto.Device.Contracts;
using TLAuto.Device.Extension.Core;
#endregion

namespace TLAuto.Device.ServerHost.IocService
{
    public partial class WcfTLAutoDeviceIocService : ITLAutoDevicePushIocService
    {
        public void RegistControlDeviceEx(string key, ControlInfo controlInfo, ITLAutoDevicePushCallback callBack)
        {
            TLDeviceExtensionsService.Instance.RegistControlDeviceEx(key, controlInfo, callBack);
        }

        public void UnRegistControlDeviceEx(string key, string serviceKey)
        {
            TLDeviceExtensionsService.Instance.UnRegistControlDeviceEx(key, serviceKey);
        }
    }
}