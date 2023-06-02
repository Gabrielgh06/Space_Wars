using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaDeBatala.GameLogic
{
    public class Enemy : GameObject
    {
        public Enemy(Size bounds, Graphics screenPainter, Point position) : base(bounds, screenPainter)
        {
            this.Left = position.X;
            this.Top = position.Y;
            this.Speed = 5;
            this.Sound = Media.ArenaDeBatalha_ObjetosDoJogo_Resources_exploshion_short;
        }

        public override Bitmap GetSprite()
        {
            return Media.inimigo;
        }

        public override void UpdateObject()
        {
            this.MoveDown();
            base.UpdateObject();
        }
    }
}
