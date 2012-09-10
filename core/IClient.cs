using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace core
{
    interface IClient
    {
        ushort ID { get; }
        IPAddress ExternalIP { get; set; }
        String DNS { get; set; }
        Guid Guid { get; set; }
        ushort FileCount { get; set; }
        ushort DataPort { get; set; }
        IPAddress NodeIP { get; set; }
        ushort NodePort { get; set; }
        String Name { get; set; }
        String OrgName { get; }
        String Version { get; }
        IPAddress LocalIP { get; set; }
        bool Browsable { get; set; }
        byte Age { get; set; }
        byte Sex { get; set; }
        byte Country { get; set; }
        String Region { get; set; }
        bool FastPing { get; set; }
        Level Level { get; set; }
        ushort Vroom { get; set; }
        bool Ghosting { get; set; }
        List<String> IgnoreList { get; set; }
        Font Font { get; set; }
        bool CustomClient { get; set; }
        List<String> CustomClientTags { get; set; }
        bool Muzzled { get; set; }
        String CustomName { get; set; }
    }
}
