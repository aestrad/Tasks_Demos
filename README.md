# Tasks_Demos

taken from docs.microsoft.com example.

One remark:

The Wait method needs to be called to ensure that all threads have completed. 

[WhenAll](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.whenall?view=net-6.0)

Yes, it is possible to use the Task.WhenAll method in combination with the Managed Extensibility Framework (MEF) to run tasks.

To use the Task.WhenAll method with MEF, you will need to create a class that represents a task and exports it using the Export attribute. You can then import the tasks using the ImportMany attribute and pass them to the Task.WhenAll method.

Here is an example of how you can use the Task.WhenAll method with MEF to run tasks:

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

// Task 1
[Export]
public class MyTask1
{
    public async Task DoWorkAsync()
    {
        // Perform task 1
        // ...
    }
}

// Task 2
[Export]
public class MyTask2
{
    public async Task DoWorkAsync()
    {
        // Perform task 2
        // ...
    }
}

public class MyClass
{
    [ImportMany]
    public IEnumerable<Lazy<Task>> Tasks { get; set; }

    public async Task DoWorkAsync()
    {
        // Wait for all tasks to complete
        await Task.WhenAll(Tasks.Select(t => t.Value));

        // All tasks have completed
        // ...
    }
}
