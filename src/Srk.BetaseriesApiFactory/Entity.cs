
namespace Srk.BetaseriesApiFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Entity
    {
        public Entity()
        {
            this.Fields = new Dictionary<string, EntityField>();
            this.UseBaseResponse = true;
        }

        public string Name { get; set; }
        public string ItemName { get; set; }

        public bool HasOneChild { get; set; }
        public bool HasMultipleChild { get; set; }

        public Entity ItemEntity { get; set; }

        public Dictionary<string, EntityField> Fields { get; set; }

        public override string ToString()
        {
            return "Entity " + this.Name + " "
                + (HasOneChild ? ("Contains 1 " + this.ItemName) : "")
                + (HasMultipleChild ? ("Contains * " + this.ItemName) : "");
        }

        public string Inherit { get; set; }

        public bool UseBaseResponse { get; set; }
    }
}

/*
comment
comment:
  - id: string
  - user_id: integer
  - login: string
  - avatar: url
  - date: datetime
  - text: string
  - inner_id: integer
  - in_reply_to: integer

comments: comment*
comment:
  - id: string
  - user_id: integer
  - login: string
  - avatar: url
  - date: datetime
  - text: string
  - inner_id: integer
  - in_reply_to: integer



characters: character*
character:
  - id: integer
  - show_id: integer
  - name: string
  - role: string
  - actor: string
  - picture: url
  - description: string


pictures: picture*
picture:
  - id: integer
  - show_id: integer
  - login_id: integer
  - url: url
  - width: width
  - height: height
  - date: datetime
  - picked: none|banner|show

messages: message*
message:
  - id: integer
  - message_id: integer
  - inner_id: integer
  - sender:
    - id: integer
    - login: string
  - recipient:
    - id: integer
    - login: string
  - date: datetime
  - title: string
  - text: text
  - unread: bool
  - has_unread: bool










*/
