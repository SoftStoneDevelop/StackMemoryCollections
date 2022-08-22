# Readme:
Collections for memory reuse by stack type.

## Usage:
Stack<T>:
```C#

unsafe
{
    using (var memory = new StackMemory(16)//allocate 12 bytes
    {
        using var stack = new Stack<int>(3, &memory);//Reserve 8 bytes for the stack
	//memory.Current adress is memory.Start + 12 bytes
	stack.Push(12);
	stack.Push(23);
	stack.SetAvailableMemoryCurrentUsed();//free 4 bytes
	//memory.Current adress is memory.Start + 8 bytes, 8 bytes free in memory
		
	using var stack2 = new Stack<int>(2, &memory);//Reserve 8 bytes for the stack2
    }
}

```

## Benchmark results:

![Stack](/BenchmarkResults/stackBench.png)
