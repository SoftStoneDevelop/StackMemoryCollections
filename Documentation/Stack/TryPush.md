# TryPush(in {ItemType}) Method

## Definition
Tries to push an element to the top of the stack.

```C#
public bool TryPush(in {ItemType} item)
```

## Parameters
`item {ItemType}`

Inserted element


## Returns
`bool`

True if element is successfully inserted otherwise False
If the instance was created using a constructor on a memory and no place for the element in the collection then: try allocate memory from `memory`.
