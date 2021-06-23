namespace BomberPerson
{
    abstract class AutoDestroy : LevelObject
    {
        //Clase padre de Fire y Bomb
        public float Duration { get; protected set; }

        protected float SetTime;

        protected AutoDestroy(Level level, float tileSize, int positionX, int positionY) : base(level, tileSize, positionX, positionY)
        {
            Level = level;

            SetTime = Round.GetElapsedTime();

            Level.Round.AddToUpdate(this);
        }

        public override void Update()
        {
            WaitAndDestroy();
        }

        protected void WaitAndDestroy()
        {
            if (Round.GetElapsedTime() - SetTime > Duration) Explode();
        }
        
        public virtual void Explode()
        {
            Level.Round.RemoveFromUpdate(this);
            base.Destroy();
        }
    }
}
