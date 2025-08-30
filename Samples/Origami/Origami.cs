using System.Collections.Concurrent;
using System.Drawing;

using Origami.Components;
using Origami.Utils;

using Prowl.PaperUI;

namespace Origami;

public interface IComponent
{
    public void ResetComponent();
}

public abstract class Component<T>  : IComponent
    where T : Component<T>
{
    public ElementBuilder ElementBuilder;
    public T Create()
    {
        ElementBuilder = Origami.Gui.Box(PaperId.Next()).BackgroundColor(Color.Black);
        return OnCreated();
    }

    public abstract void Finish();

    protected abstract T OnCreated();
    public abstract T Draw();
    public abstract void ResetComponent();
}

public abstract class PersistentComponent
{
    private object _State;
}

public static class Origami
{
    private static bool isInitialized = false;
    private static bool isFrameStarted = false;
    public static Paper Gui { get; set; }
    private static ConcurrentDictionary<Type, List<IComponent>> ComponentPool { get; set; }
    private static ConcurrentDictionary<Type, int> PoolSizeLimits { get; set; }
    private static ConcurrentDictionary<Type, Func<IComponent>> Constructors { get; set; }
    // private static ConcurrentDictionary<Type, List<IState>> StateStorage { get; set; }
    private static ConcurrentDictionary<Type, List<ulong>> IdStorage { get; set; }
    private static ConcurrentDictionary<Type, int> IndexStorage { get; set; }
    // private static ConcurrentDictionary<Type, Type> StateTypeMap { get; set; }
    public static void Init(Paper paper)
    {
        if (isInitialized) throw new InvalidOperationException("Origami has already been initialized. You cannot do this twice");
        isInitialized = true;
        ComponentPool = new ConcurrentDictionary<Type, List<IComponent>>();
        Constructors = new ConcurrentDictionary<Type, Func<IComponent>>();
        PoolSizeLimits = new ConcurrentDictionary<Type, int>();
        // StateStorage = new ConcurrentDictionary<Type, List<IState>>();
        IdStorage = new ConcurrentDictionary<Type, List<ulong>>();
        IndexStorage = new ConcurrentDictionary<Type, int>();
        // StateTypeMap = new ConcurrentDictionary<Type, Type>();

        Gui = paper;

        RegisterStateComponent<Button>();
        RegisterStatelessComponent<AccordianItem>();
    }

    public static void RegisterStatelessComponent<T>(int poolSizeLimit = 128) where T : IComponent, new()
    {
        if (ComponentPool.ContainsKey(typeof(T)))
            throw new InvalidOperationException($"Component {typeof(T)} was already registered. Please check your code");

        Constructors[typeof(T)] = () => new T();
        ComponentPool[typeof(T)] = new();
        PoolSizeLimits[typeof(T)] = poolSizeLimit;
    }

    public static void RegisterStateComponent<T>(int poolSizeLimit = 128) where T : IComponent, new()
    {
        RegisterStatelessComponent<T>();

        // StateStorage[typeof(T)] = new List<IState>();
        IdStorage[typeof(T)] = new List<ulong>();
        IndexStorage[typeof(T)] = 0;
        // StateTypeMap[typeof(T)] = typeof(T2);
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

    }

    public static T Component<T>() where T : Component<T>, new()
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

        var newComponent = ((T)componentList[index]);
        newComponent.Create();

        if(IdStorage[typeof(T)][index] != newComponent.ElementBuilder._element.ID)
        {
            newComponent.ResetComponent();
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
