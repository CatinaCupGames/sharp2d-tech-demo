using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharp2D;

namespace TechDemo
{
    public class Bullet : PhysicsSprite
    {
        public Sprite owner;
        private int direction;
        private float xvel, yvel;
        public Bullet(Sprite owner, int direction)
        {
            this.owner = owner;
            this.direction = direction;
        }

        public override string Name
        {
            get { return "bullet"; }
        }

        public override string AnimationConfigPath
        {
            get
            {
                return "sprites/bullet/bullet.conf";
            }
        }

        protected override void BeforeDraw()
        {
        }

        protected override void OnDisplay()
        {
        }

        protected override void OnLoad()
        {
            Texture = Texture.NewTexture("sprites/player/player.png");
            Texture.LoadTextureFromFile();

            Width = Texture.TextureWidth;
            Height = Texture.TextureHeight;

            base.OnLoad();

            OnCollision += Bullet_OnCollision;
        }

        void Bullet_OnCollision(object sender, EventArgs e)
        {
            OnCollisionEventArgs args = (OnCollisionEventArgs)e;

            if (args.Collider == owner || args.With == owner)
                return;

            if (CurrentWorld is BasicWorld)
            {
                ((BasicWorld)CurrentWorld).RemoveSprite(this);
                if (args.Collider is Player)
                {
                    ((Player)args.Collider).Player_OnCollision(sender, e);
                }
                if (args.With is Player)
                {
                    ((Player)args.With).Player_OnCollision(sender, e);
                }
            }
        }

        public override void Update()
        {
            base.Update();

            if (Y > 45f * 16f || Y < -6f * 16f || X > 83f * 16f || X < -29f * 16f)
            {
                ((BasicWorld)CurrentWorld).RemoveSprite(this);
                return;
            }

            PhysicsImpulse();
            PhysicsMove();
        }

        public void PhysicsMove()
        {
            X += xvel * Player.SlowMoModifier;
            Y += yvel * Player.SlowMoModifier;
        }

        public void PhysicsImpulse()
        {
            xvel += (Player.ACCELERATION * direction) * Player.SlowMoModifier;

            xvel -= xvel * Player.AIR_FRICTION;
        }
    }
}
