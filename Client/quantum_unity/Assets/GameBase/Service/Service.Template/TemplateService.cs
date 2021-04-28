using System;
using System.Collections.Generic;
using Service.Net;

namespace Service.Template
{
    public class TemplateService
    {
        Dictionary<string, ITemplate> _templates;

        
        public TemplateService()
        {
            _templates = new Dictionary<string, ITemplate>();
        }

      
        public void Update()
        {
            foreach (var t in _templates)
            {
                t.Value.Update();
            }
        }


        public void Destroy()
        {
            foreach (var t in _templates)
            {
                t.Value.Destroy();
            }
        }


        public bool AddTemplate(string id, ITemplate template)
        {
            _templates.Add(id, template);
            return true;
        }


        public bool PushItem(string pk)
        {
            foreach (var t in _templates)
            {
                t.Value.PushItem(pk);
            }

            return true;
        }


        public bool GetItem(string pk, string sk)
        {
            foreach (var t in _templates)
            {
                t.Value.GetItem(pk, sk);
            }

            return true;
        }


        public bool ConnectNewPlayer(GamePlayer player)
        {
            foreach (var t in _templates)
            {
                t.Value.ConnectNewPlayer(player);
            }

            return true;
        }

        public bool DisconnectPeer(GamePlayer player)
        {
            foreach (var t in _templates)
            {
                t.Value.DisconnectPlayer(player);
            }

            return true;
        }
    }
}