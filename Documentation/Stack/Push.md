# Push(in {ItemType}) Method

## Definition
Inserts an element at the top of the Stack.

```C#
public void Push(in {ItemType} item)
```

## Parameters
`item {ItemType}`

Inserted element

## Exceptions

```C#
Exception
```
If there is no place for the element in the collection

## Remarks

If the instance was created using a constructor on a memory and no place for the element in the collection then: allocate memory from `memory`.
