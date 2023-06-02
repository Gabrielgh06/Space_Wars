using System.Drawing;
using System.IO;
using System.Media;

namespace ArenaDeBatala.GameLogic
{
    public abstract class GameObject
    {
        #region Game Object Properties
        public Bitmap Sprite { get; set; } // Imagens
        public bool Active { get; set; }
        public int Speed { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get { return this.Sprite.Width; } }
        public int Height { get { return this.Sprite.Height; } }
        public Size Bounds { get; set; } // Limites
        public Rectangle Rectangle { get; set; }
        public Stream Sound { get; set; }
        public Graphics ScreenPainter { get; set; }
        private SoundPlayer soundPlayer { get; set; }
        #endregion


        #region Game Object Methods

        public abstract Bitmap GetSprite();

        public  GameObject(Size bounds, Graphics ScreenPainter) // Cria os Objetos
        {
            this.Bounds = bounds;
            this.ScreenPainter = ScreenPainter;
            this.Active = true;
            this.soundPlayer = new SoundPlayer();
            this.Sprite = this.GetSprite();
            this.Rectangle = new Rectangle(this.Left, this.Top, this.Width, this.Height);
        }

        public virtual void UpdateObject() // Atualiza o objeto
        {
            this.Rectangle = new Rectangle(this.Left, this.Top, this.Width, this.Height);
            this.ScreenPainter.DrawImage(this.Sprite, this.Rectangle);
        }

        public virtual void MoveLeft()
        {
            if (this.Left > 0)
            {
                this.Left -= this.Speed;
            }
        }

        public virtual void MoveRight()
        {
            if (this.Left < this.Bounds.Width - this.Width)
            {
                this.Left += this.Speed;
            }
        }

        public virtual void MoveDown()
        {
            this.Top += this.Speed;
            
        }

        public virtual void MoveUp()
        {
            this.Top -= this.Speed;
            
        }

        public bool IsOutOfBounds() // Verifica se o objeto esta dentro dos limites
        {
            return
                (this.Top > this.Bounds.Height + this.Height) ||
                (this.Top < -this.Height) ||
                (this.Left > this.Bounds.Width + this.Width) ||
                (this.Left < -this.Width);
        }

        public void PlaySound()
        {
            soundPlayer.Stream = this.Sound;
            soundPlayer.Play();
        }

        public bool IsCollidingWith(GameObject gameObject)
        {
            if (this.Rectangle.IntersectsWith(gameObject.Rectangle))
            {
                this.PlaySound();
                return true;
            }
            else return false;
        }

        public void Destroy()
        {
            this.Active = false;
        }

        #endregion
    }
}
