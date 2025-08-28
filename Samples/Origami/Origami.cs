using System.Collections.Concurrent;
using System.Drawing;

using Origami.Components;
using Origami.Utils;

using Prowl.PaperUI;

namespace Origami;

public interface IComponent
{
    public void Reset();
}

public abstract class Component<T> : IComponent where T : Component<T>
{
    public ElementBuilder _elementBuilder;
    public T Create()
    {
        _elementBuilder = Origami.Gui.Box(PaperId.Next()).BackgroundColor(Color.Black);
        return OnCreated();
    }

    public abstract void Finish();

    protected abstract T OnCreated();
    public abstract T Draw();
    public abstract void Reset();
}

public static class Origami
{
    private static bool isInitialized = false;
    public static Paper Gui { get; set; }
    private static ConcurrentDictionary<Type, ConcurrentQueue<IComponent>> ComponentPool { get; set; }
    private static ConcurrentDictionary<Type, int> PoolSizeLimits { get; set; }
    private static ConcurrentDictionary<Type, Func<IComponent>> Constructors { get; set; }

    public static void Init(Paper paper)
    {
        if (isInitialized) throw new InvalidOperationException("Origami has already been initialized. You cannot do this twice");
        isInitialized = true;
        ComponentPool = new ConcurrentDictionary<Type, ConcurrentQueue<IComponent>>();
        Constructors = new ConcurrentDictionary<Type, Func<IComponent>>();
        PoolSizeLimits = new ConcurrentDictionary<Type, int>();

        Gui = paper;

        RegisterComponent<Button>();
        RegisterComponent<AccordianItem>();
    }

    public static void RegisterComponent<T>(int poolSizeLimit = 128) where T : IComponent, new()
    {
        if (Constructors.ContainsKey(typeof(T)))
            throw new InvalidOperationException($"Component {typeof(T)} was already registered. Please check your code");

        Constructors[typeof(T)] = () => new T();
        ComponentPool[typeof(T)] = new ConcurrentQueue<IComponent>();
        PoolSizeLimits[typeof(T)] = poolSizeLimit;
    }

    public static T Component<T>() where T : Component<T>, new()
    {
        if (!isInitialized) throw new InvalidOperationException("Origami has not been initialized. Call Origami.Init() first.");
        if (!ComponentPool.TryGetValue(typeof(T), out var queue))
            throw new InvalidOperationException($"Component {typeof(T)} is not registered. Call Origami.RegisterComponent<{typeof(T)}>() first.");

        if (queue.TryDequeue(out var component))
            return ((T)component).Create();

        return ((T)Constructors[typeof(T)]()).Create();
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
        if (!ComponentPool.TryGetValue(typeof(T), out var queue))
            throw new InvalidOperationException($"Component {typeof(T)} is not registered.");
        return queue.Count;
    }

    public static void ReturnToPool<T>(T component)  where T : Component<T>
    {
        component.Reset();
        ComponentPool[typeof(T)].Enqueue(component);

        if (ComponentPool[typeof(T)].Count >= PoolSizeLimits[typeof(T)])
            throw new InvalidOperationException(
                $"{typeof(T)} has exceeded the maximum pool size. If you need a larger size, call Origami.SetPoolSizeLimit<{typeof(T)}>(newPoolSize)");
    }
}
