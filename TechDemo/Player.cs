using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using Sharp2D;
using Sharp2D.Game.Worlds;
using Sharp2D.Core.Interfaces;

namespace TechDemo
{
    public class Player : PhysicsSprite
    {
        public const float JUMP_ACCEL = 6f;
        public const float GRAVITY = 0.7f;
        public const float ACCELERATION = 8f;
        public const float AIR_ACCELERATION = 0.7f;
        public const float GROUND_FRICTION = 0.4f;
        public const float GROUND_FRICTION_ADD = 0.1f;
        public const float AIR_FRICTION = 0.05f;
        public const float SLOW_MO = 0.3f;
        public const int TOTAL_SLOWDOWN_TIME = 25 * 2;

        public static float SlowMoModifier = 1f;
        public static List<Sprite> slowMos = new List<Sprite>();

        private bool disabled = false;

        public TiledWorld tworld;
        public bool isOnGround = false;
        public float xvel, yvel;
        public bool player2;
        
        
        private int fired = 3;
        private bool firePress = false;

        public int lastDirection = 0;
        private bool slowmo = false;
        public int SlowDownTime = TOTAL_SLOWDOWN_TIME;

        public bool HasController
        {
            get
            {
                if (!player2)
                {
                    return GamePad.GetState(0).IsConnected;
                }
                else
                {
                    return GamePad.GetState(1).IsConnected;
                }
            }
        }

        public override string Name
        {
            get { return "player"; }
        }

        public override string AnimationConfigPath
        {
            get
            {
                return "sprites/player/player.conf";
            }
        }

        protected override void OnDisplay()
        {
            SlowMoBar bar = new SlowMoBar(this);
            bar.X = X;
            bar.Y = Y - ((Width / 2f) + 16f);

            ((SpriteWorld)CurrentWorld).AddSprite(bar);

            Attach(bar);
        }

        protected override void OnLoad()
        {
            Texture = Texture.NewTexture("sprites/player/player.png");
            Texture.LoadTextureFromFile();
            using (Graphics g = Graphics.FromImage(Texture.Bitmap))
            {
                using (Brush b = new SolidBrush(Color.Black))
                {
                    string s = "1";
                    if (player2)
                    {
                        s = "2";
                    }
                    g.DrawString(s, new Font("Arial", 12f, FontStyle.Bold, GraphicsUnit.Point), b, new PointF(24f, 22f));
                }
            }

            Width = Texture.TextureWidth;
            Height = Texture.TextureHeight;

            base.OnLoad();

            OnCollision += Player_OnCollision;
        }

        public void Player_OnCollision(object sender, EventArgs e)
        {
            OnCollisionEventArgs args = (OnCollisionEventArgs)e;

            if (args.Collider is Bullet)
            {
                Bullet b = (Bullet)args.Collider;
                if (b.owner == this)
                    return;

                disabled = true;
                
                string text = "WON!";
                if (player2)
                    text = "Player 1 " + text;
                else
                    text = "Player 2 " + text;

                WinText texts = new WinText(text);
                ((SpriteWorld)CurrentWorld).AddSprite(texts);
            }
            if (args.With is Bullet)
            {
                Bullet b = (Bullet)args.With;
                if (b.owner == this)
                    return;

                disabled = true; 
                
                string text = "WON!";
                if (player2)
                    text = "Player 1 " + text;
                else
                    text = "Player 2 " + text;

                WinText texts = new WinText(text);
                ((SpriteWorld)CurrentWorld).AddSprite(texts);
            }
        }

        protected override void BeforeDraw()
        {
        }

        public override void OnAddedToWorld(Sharp2D.Core.World w)
        {
            base.OnAddedToWorld(w);

            if (w is TiledWorld)
                tworld = (TiledWorld)w;
        }

        public override void Update()
        {
            base.Update();

            if (Y > 45f * 16f)
            {
                Y = -5f * 16f;
            }
            if (Y < -6f * 16f)
            {
                Y = 43f * 16f;
            }
            if (X > 83f * 16f)
            {
                X = -28f * 16f;
            }
            if (X < -29f * 16f)
            {
                X = 82f * 16f;
            }

            PhysicsImpulse();

            isOnGround = false;

            Hitbox.ForEachCollidable(delegate(ICollidable c)
            {
                if (c.Hitbox == null)
                    return;
                if ((c.Y - c.Height / 2f) >= (Y + Height / 2f))
                {
                    var result = Hitbox.CheckCollision(this, c, new Vector2(0, yvel * SlowMoModifier));
                    if (result.WillIntersect || result.Intersecting)
                    {
                        isOnGround = true;
                    }
                }
            });

            PhysicsMove();

            if (HasController)
                CheckInputGamepad();
            else
                CheckInputKeyboard();
        }

        public void PhysicsMove()
        {
            if (!slowmo)
            {
                X += xvel * SlowMoModifier;
                Y += yvel * SlowMoModifier;
            }
            else
            {
                X += xvel;
                Y += yvel;
            }
        }

        public override string ToString()
        {
            if (!player2)
                return "Player 1";
            else
                return "Player 2";
        }

        public Vector2 GetInput()
        {
            if (disabled)
                return new Vector2(0, 0);

            if (!player2)
            {
                if (!HasController)
                {
                    var state = Keyboard.GetState();
                    int x = state.IsKeyDown(Key.D) ? 1 : state.IsKeyDown(Key.A) ? -1 : 0;
                    int y = state.IsKeyDown(Key.W) ? -1 : state.IsKeyDown(Key.S) ? 1 : 0;

                    return new Vector2(x, y);
                }
                var gstate = GamePad.GetState(0);
                Vector2 toreturn = gstate.ThumbSticks.Left;
                toreturn.Y = gstate.Buttons.A == ButtonState.Pressed ? -1f : 0f;
                return toreturn;
            }
            else
            {
                if (!HasController)
                {
                    var state = Keyboard.GetState();
                    int x = state.IsKeyDown(Key.Right) ? 1 : state.IsKeyDown(Key.Left) ? -1 : 0;
                    int y = state.IsKeyDown(Key.Up) ? -1 : state.IsKeyDown(Key.Down) ? 1 : 0;

                    return new Vector2(x, y);
                }
                var gstate = GamePad.GetState(1); 
                Vector2 toreturn = gstate.ThumbSticks.Left;
                toreturn.Y = gstate.Buttons.A == ButtonState.Pressed ? -1f : 0f;
                return toreturn;
            }
        }

        public void CheckInputGamepad()
        {
            if (disabled)
                return;

            var gstate = GamePad.GetState(player2 ? 1 : 0);

            slowmo = gstate.Triggers.Left > 0.3f && SlowDownTime > 0;
            if (slowmo)
            {
                SlowMoModifier = SLOW_MO;
                SlowDownTime--;
                if (!slowMos.Contains(this))
                    slowMos.Add(this);
            }
            else
            {
                slowMos.Remove(this);
            }

            if (slowMos.Count == 0)
                SlowMoModifier = 1f;

            if (fired > 0)
            {
                if (gstate.Triggers.Right > 0.3f && !firePress)
                {
                    fired--;
                    firePress = true;
                    Bullet bullet = new Bullet(this, lastDirection);
                    if (lastDirection == 1)
                        bullet.X = (X + (Width / 2f));
                    else
                        bullet.X = (X - (Width / 2f));
                    bullet.Y = Y;
                    Program.world.AddSprite(bullet);
                }
                else if (gstate.Triggers.Right <= 0.3f) firePress = false;
            }
        }

        public void CheckInputKeyboard()
        {
            if (disabled)
                return;

            var state = Keyboard.GetState();

            if (!player2)
            {
                slowmo = state.IsKeyDown(Key.Space) && SlowDownTime > 0;
                if (slowmo)
                {
                    SlowMoModifier = SLOW_MO;
                    SlowDownTime--;
                    if (!slowMos.Contains(this))
                        slowMos.Add(this);
                }
                else
                {
                    slowMos.Remove(this);
                }

                if (slowMos.Count == 0)
                    SlowMoModifier = 1f;

                if (fired > 0)
                {
                    if (state.IsKeyDown(Key.LShift) && !firePress)
                    {
                        fired--;
                        firePress = true;
                        Bullet bullet = new Bullet(this, lastDirection);
                        if (lastDirection == 1)
                            bullet.X = (X + (Width / 2f));
                        else
                            bullet.X = (X - (Width / 2f));
                        bullet.Y = Y;
                        Program.world.AddSprite(bullet);
                    }
                    else if (!state.IsKeyDown(Key.LShift)) firePress = false;
                }
            }
            else
            {
                slowmo = state.IsKeyDown(Key.Enter) && SlowDownTime > 0;
                if (slowmo)
                {
                    SlowMoModifier = SLOW_MO;
                    SlowDownTime--;
                    if (!slowMos.Contains(this))
                        slowMos.Add(this);
                }
                else
                {
                    slowMos.Remove(this);
                }

                if (slowMos.Count == 0)
                    SlowMoModifier = 1f;

                if (fired > 0)
                {
                    if (state.IsKeyDown(Key.RShift) && !firePress)
                    {
                        fired--;
                        firePress = true;
                        Bullet bullet = new Bullet(this, lastDirection);
                        if (lastDirection == 1)
                            bullet.X = (X + (Width / 2f));
                        else
                            bullet.X = (X - (Width / 2f));
                        bullet.Y = Y;
                        Program.world.AddSprite(bullet);
                    }
                    else if (!state.IsKeyDown(Key.RShift)) firePress = false;
                }
            }
        }

        public void PhysicsImpulse()
        {

            float mod = slowmo ? 1f : SlowMoModifier;

            yvel += (GRAVITY * mod);

            if (isOnGround)
            {
                if(yvel > 0)
                    yvel = 0;
            }

            Vector2 left = GetInput();
            if (left.LengthSquared < 0.3f)
                left = new Vector2(0, 0);

            if (left.X > 0)
                lastDirection = 1;
            else if (left.X < 0)
                lastDirection = -1;

            if (isOnGround)
            {
                xvel += left.X * (ACCELERATION * mod);
            }
            else
            {
                xvel += left.X * (AIR_ACCELERATION * mod);
            }
            
            if (left.Y < 0)
            {
                if(isOnGround)
                yvel -= JUMP_ACCEL;
            }

            ApplyFriction();
        }

        public void ApplyFriction()
        {
            float frict;
            if (isOnGround)
            {
                frict = xvel * GROUND_FRICTION;
                if (frict > 0.0)
                {
                    frict += GROUND_FRICTION_ADD;
                    if (frict > xvel) xvel = 0.0f;
                    else xvel -= frict;
                }
                else
                {
                    frict -= GROUND_FRICTION_ADD;
                    if (frict < xvel) xvel = 0.0f;
                    else xvel -= frict;
                }
            }
            else
            {
                xvel -= xvel * AIR_FRICTION;
            }
        }
    }
}
