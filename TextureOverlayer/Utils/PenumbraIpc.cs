using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dalamud.Configuration;
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
        private readonly GetModDirectory _getModDirectory = new GetModDirectory(pluginInterface);


        private GetModPath _getModPath = new GetModPath(pluginInterface);
        public Dictionary<string, string> Modlist
        {

            get {return modList;} 
            set => this.modList = new GetModList(pluginInterface).Invoke();
        }

        public String GetModDirectory()
        {
            return _getModDirectory.Invoke();
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


           var files = Directory.GetFiles($"{Service.penumbraApi.GetModDirectory().ToString()}\\{modName}" , "*.tex", SearchOption.AllDirectories).ToList();
           
           return files;
       }
       
       public String setupFolderStructure()
       {
           if (Directory.Exists(Service.penumbraApi.GetModDirectory().ToString() + "\\TextureOverlayer"))
           {
               return Service.penumbraApi.GetModDirectory().ToString() + "\\TextureOverlayer";
           }
           else
           {
               try
               {
                   System.IO.Directory.CreateDirectory(Service.penumbraApi.GetModDirectory().ToString() + "\\TextureOverlayer");
                   return Service.penumbraApi.GetModDirectory().ToString() + "\\TextureOverlayer";
               }
               catch (Exception e)
               {
                   Service.Log.Error(e.ToString());
               }
            
           }

           return String.Empty;
       }

       
        public void Dispose()
        {
            /*modAddedEventSubscriber.Dispose();
            modDeletedEventSubscriber.Dispose();
            modMovedEventSubscriber.Dispose();*/
        }




    }
}
