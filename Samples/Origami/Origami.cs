using System.Collections.Concurrent;
using System.Drawing;

using Prowl.PaperUI;

namespace OrigamiUI;

public interface IComponent
{
    // public void ResetComponent();
}

public interface IState
{

}

public interface IPersistentState
{
    public void Reset();
}

public abstract class Component<T>  : IComponent
    where T : Component<T>
{
    public ElementBuilder ElementBuilder;
    public T Create(string id)
    {
        ElementBuilder = Origami.Gui.Box(id).BackgroundColor(Color.Black);
        return OnCreated();
    }

    public ElementBuilder GetBuilder() => ElementBuilder;
    public abstract ElementBuilder DrawDefault();

    protected abstract T OnCreated();
    public abstract T Draw();
}

public abstract class PersistentComponent
{
    private object _State;
}

public static class Origami
{
    private static bool isInitialized = false;
    private static bool isFrameStarted = false;
    private static bool isFrameEnded = false;
    public static Paper Gui { get; set; }
    private static ConcurrentDictionary<Type, List<IComponent>> ComponentPool { get; set; }
    private static ConcurrentDictionary<Type, int> PoolSizeLimits { get; set; }
    private static ConcurrentDictionary<Type, Func<IComponent>> Constructors { get; set; }
    private static ConcurrentDictionary<Type, List<ulong>> IdStorage { get; set; }
    private static ConcurrentDictionary<Type, int> IndexStorage { get; set; }
    private static ConcurrentDictionary<Type, int> NumberRenderedLastFrame { get; set; }
    public static void Init(Paper paper)
    {
        if (isInitialized) throw new InvalidOperationException("Origami has already been initialized. You cannot do this twice");
        isInitialized = true;
        ComponentPool = new ConcurrentDictionary<Type, List<IComponent>>();
        Constructors = new ConcurrentDictionary<Type, Func<IComponent>>();
        PoolSizeLimits = new ConcurrentDictionary<Type, int>();
        IdStorage = new ConcurrentDictionary<Type, List<ulong>>();
        IndexStorage = new ConcurrentDictionary<Type, int>();
        NumberRenderedLastFrame = new ConcurrentDictionary<Type, int>();

        Gui = paper;

        RegisterComponent<Button>();
        RegisterComponent<AccordianItem>();
        RegisterComponent<Dropdown>();
        RegisterComponent<Modal>();
    }

    public static void RegisterComponent<T>(int poolSizeLimit = 128) where T : IComponent, new()
    {
        if (ComponentPool.ContainsKey(typeof(T)))
            throw new InvalidOperationException($"Component {typeof(T)} was already registered. Please check your code");

        Constructors[typeof(T)] = () => new T();
        ComponentPool[typeof(T)] = new();
        PoolSizeLimits[typeof(T)] = poolSizeLimit;
        IdStorage[typeof(T)] = new List<ulong>();
        IndexStorage[typeof(T)] = 0;
        NumberRenderedLastFrame[typeof(T)] = 0;
    }

    public static void BeginFrame()
    {
        isFrameStarted = true;

        foreach (Type key in IndexStorage.Keys)
        {
            IndexStorage[key] = 0;
        }
    }

    public static void EndFrame()
    {
        foreach (Type key in IndexStorage.Keys)
        {
            if (!key.IsAssignableTo(typeof(IPersistentState))) continue;
            if (NumberRenderedLastFrame[key] > IndexStorage[key])
            {
                for (int i = IndexStorage[key]; i < NumberRenderedLastFrame[key]; i++)
                {
                    ((IPersistentState)ComponentPool[key][i]).Reset();
                }
            }

            NumberRenderedLastFrame[key] = IndexStorage[key];
        }
    }

    public static T Component<T>(string id) where T : Component<T>, new()
    {
        if (!isInitialized) throw new InvalidOperationException("Origami has not been initialized. Call Origami.Init() first.");
        if (!isFrameStarted)
            throw new InvalidOperationException(
                "You need to call Origami.BeginFrame() before you can create components.");
        if (!ComponentPool.TryGetValue(typeof(T), out var componentList))
            throw new InvalidOperationException($"Component {typeof(T)} is not registered. Call Origami.RegisterComponent<{typeof(T)}>() first.");
        if (componentList.Count >= PoolSizeLimits[typeof(T)])
            throw new InvalidOperationException(
                $"{typeof(T)} has exceeded the maximum pool size. If you need a larger size, call Origami.SetPoolSizeLimit<{typeof(T)}>(newPoolSize)");

        int index = IndexStorage[typeof(T)];
        IndexStorage[typeof(T)]++;

        if (index >= ComponentPool[typeof(T)].Count)
        {
            ComponentPool[typeof(T)].Add(Constructors[typeof(T)]());
            IdStorage[typeof(T)].Add(0);
        }

        var newComponent = (T)componentList[index];
        newComponent.Create(id);

        if(IdStorage[typeof(T)][index] != newComponent.ElementBuilder._element.ID)
        {
            if(typeof(T).IsAssignableTo(typeof(IPersistentState))) ((IPersistentState)newComponent).Reset();
            IdStorage[typeof(T)][index] = newComponent.ElementBuilder._element.ID;
        }

        return newComponent;
    }

    public static void SetPoolSizeLimit<T>(int size) where T : Component<T>
    {
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), "Pool size limit must be a positive integer.");
        if (!PoolSizeLimits.ContainsKey(typeof(T)))
            throw new InvalidOperationException($"Component {typeof(T)} is not registered. Call Origami.RegisterComponent<{typeof(T)}>() first.");

        PoolSizeLimits[typeof(T)] = size;
    }

    public static int GetPoolSize<T>() where T : Component<T>
    {
        if (!ComponentPool.TryGetValue(typeof(T), out var createdComponents))
            throw new InvalidOperationException($"Component {typeof(T)} is not registered.");
        return createdComponents.Count;
    }

    public static void ReturnToPool<T>(T component)  where T : Component<T>
    {
        // component.ResetComponent();
        // ComponentPool[typeof(T)].Enqueue(component);
    }
}
