# Readme:
Collections for memory reuse by stack type.

Supported collections(link to API description):
- Stack

Common Types:
- StackMemory

## Usage:
```C#

unsafe
{
    using (var memory = new Struct.StackMemory(16)//allocate 16 bytes
    {
        using var stack = new Stack<int>(3, &memory);//Reserve 8 bytes for the stack
	//memory.Current adress is memory.Start + 12 bytes
	stack.Push(12);
	stack.Push(23);
	stack.TrimExcess();//free 4 bytes - 1 item
	//memory.Current adress is memory.Start + 8 bytes, 8 bytes free in memory
		
	using var stack2 = new Stack<int>(2, &memory);//Reserve 8 bytes for the stack2
	//...
    }//free all memory
}

```
