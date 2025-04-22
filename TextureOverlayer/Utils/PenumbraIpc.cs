using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dalamud.Plugin;
using Penumbra.Api.Enums;
using Penumbra.Api.Helpers;
using Penumbra.Api.IpcSubscribers;
using TextureOverlayer;

namespace TextureOverlayer.Interop
{
    internal class PenumbraIpc(IDalamudPluginInterface pluginInterface) : IDisposable
    {


        private IDalamudPluginInterface _pluginInterface { get; }

        private Dictionary<string, string> modList = new();


        private GetModPath _getModPath = new GetModPath(pluginInterface);
        public Dictionary<string, string> Modlist
        {

            get {return modList;} 
            set => this.modList = new GetModList(pluginInterface).Invoke();
        }


        /*
        private readonly EventSubscriber<string> modDeletedEventSubscriber = 
        ModDeleted.Subscriber(pluginInterface, Service.penumbraApi.UpdateModList);
        private readonly EventSubscriber<string, string> modMovedEventSubscriber = 
        ModMoved.Subscriber(pluginInterface, Service.penumbraApi.UpdateModList);*/
        
       /* public  void UpdateModList(string empty)
        {
            Modlist = Service.penumbraApi.GetModList();
        }
        public  void UpdateModList(string empty, string empty1)
        {
            Modlist = Service.penumbraApi.GetModList();
        } */



       public List<String> GetTextureList(String modName)
       {
           
           var files = Directory.GetFiles($"D:\\Games\\FFXIV\\Penumbra\\{modName}" , "*.tex", SearchOption.AllDirectories).ToList();
           
           return files;
       }
       

        public void Dispose()
        {
            /*modAddedEventSubscriber.Dispose();
            modDeletedEventSubscriber.Dispose();
            modMovedEventSubscriber.Dispose();*/
        }




    }
}
