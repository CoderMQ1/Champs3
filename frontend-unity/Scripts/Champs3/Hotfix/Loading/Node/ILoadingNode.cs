using QFramework;

namespace champs3.Hotfix
{
    public interface ILoadingNode
    {
        void Initialize();
        void Enter();
        void Update();
        void Exit();
        bool CanStart();
        void Start();
        void Finish();
        float Progress();
        bool IsDone();
        
    }
    
    public abstract class AbstractLoadingNode : ILoadingNode
    {
        protected bool Initialized;
        
        public delegate void Handler();
        
        public Handler StartHandler;
        public Handler CompletHandler;
        public Handler EnterHandler;
        public Handler ExitHandler;

        public void Initialize()
        {
            if (!Initialized)
            {
                OnInitialize();
            }
        }

        protected virtual void OnInitialize()
        {
            
        }

        public void Enter()
        {
            EnterHandler?.Invoke();
            OnEnter();
 
        }

        protected virtual void OnEnter()
        {
            
        }

        public void Start()
        {
            if (CanStart())
            {
                StartHandler?.Invoke();
                OnStart();
            }
            else
            {
                LogKit.E($"{this.GetType()} is not can start");
            }
        }
        protected virtual void OnStart()
        {
            
        }


        public void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }

        public void Exit()
        {
            ExitHandler?.Invoke();
            OnExit();
        }
        protected virtual void OnExit()
        {
            
        }

        public void Finish()
        {
            CompletHandler?.Invoke();
            OnFinish();
        }

        protected virtual void OnFinish()
        {
            
        }

        public abstract float Progress();

        public abstract bool IsDone();
        
        public abstract bool CanStart();
    }
}