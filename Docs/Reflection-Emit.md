## Reflection.Emit

## 生成 Assembly

- 用 AssemblyBuilder 生成一个 Assembly
- 在 Assembly 中生成一个 Module
- 在 Module 中生成需要的类型
- 给这个类中添加属性，方法，事件等
- 使用 ILGenerator 把上面的属性，方法，事件写进去

```c#
public class Builder {
    #region Event
    public delegate void BuilderDelegate(string message);

    public event BuilderDelegate InvokeMessage;

    public virtual void OnInvokeMessage(string message) {
        if (this.InvokeMessage != null)
            this.InvokeMessage(message);
    }
    #endregion

    #region Fields
    private int firstNum, secondNum;
    public int FirstNum {
        [DebuggerStepThrough()]
        get { return this.firstNum; }
        set { this.firstNum = value; }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int SecondNum {
        get { return this.secondNum; }
        set { this.secondNum = value; }
    }

    #endregion

    #region Constructors
    public Builder(int firstnum, int secondnum) {
        this.FirstNum = firstnum;
        this.SecondNum = secondnum;
    }
    #endregion

    #region IBuilder Members

    public float Sum(int firstnum, int secondnum) {
        return firstnum + secondnum;
    }

    public float Substract(int firstnum, int secondnum) {
        return firstnum - secondnum;
    }

    public float Multiply(int firstnum, int secondnum) {
        return firstnum * secondnum;
    }

    public float Divide(int firstnum, int secondnum) {
        try {
            return firstnum / secondnum;
        } catch (DivideByZeroException ex) {
            Console.WriteLine("ZeroDivide exception : {0}", ex.Message);
            return 0;
        }
    }

    #endregion

    #region Methods

    public float GetProduct() {
        return this.Multiply(this.FirstNum, this.secondNum);
    }
    public override string ToString() {
        return string.Format("FirstNum : {0}, SecondNum : {1}", this.FirstNum, this.SecondNum);
    }

    #endregion
}
```

上面的代码，我定义了一个 IBuilder 接口，并用 Builder 类实现了它。这个方法基本拥有各种成员信息，如字段，属性，方法，事件等。

然后我们打开 ILDASM 看看生成的 IL 是什么样的。基本上它包括两个两个类

![1548232598(1)](C:\Users\maos\Desktop\1548232598(1).jpg)

1. .class interface 是 IBuilder
2. .class 是 IBuilder 的实现类 Builder

其他的方法是 Program.Main 中的代码，这里不说。

![1548232863(F:\MS\Project\DynamicProxyUtil\Docs\images\reflection-emit-whole)](C:\Users\maos\Desktop\1548232863(1).jpg)

从这个图可以清楚的看出 CLR 程序集整个结构是如何工作的。（注意，此图为 ASP.NET 下的 CLR 结构图，与 .NET Core 有区别）Application Domain 是创建 Assembly、Module、Type 的根，而 Delegate 也是一个类，它继承自 System.MultiCastDelegate 并且结构是继承自 System.ValueTypes。每个类型都维护自己的成员信息，每个成员方法或属性有自己的操作码（Opcodes），Locals（本地变量） 以及 参数（Parameters）。你可以用Locals 在方法体类定义局部变量，操作码是 IL 的指令码。

现在开始第二步，新建动态的程序集

> 注意，以下代码，均用 netcore 版本编写，最后会给出 .netframwork 版本的代码

### 1：创建动态 Assembly

```c#
// netcore version
private const string AssemblyName = "ReflectionEmitGuider.DynamicAssembly";
public AssemblyBuilder GetAssemblyBuilder() {
    var assemblyName = new AssemblyName(AssemblyName);
    return AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
}
```

创建程序集，你需要指定一个程序集唯一标识的名字。由于根部对象是 AppDomain（.net core 下没有直接暴露 API，它依赖于 Window 平台），所以跨域的时候，这种方法是没有效果的。

### 2：创建 Module

```c#
public ModuleBuilder GetModuleBuilder(AssemblyBuilder assemblyBuilder)
{
    var moduleBuilder = assemblyBuilder.DefineDynamicModule(ModuleName);
    return moduleBuilder;
}
```

Module 定义还提供了很多特性，详情可以去看 API。

### 3：创建类型

这是主要的核心操作。创建一个类，结构体，委托等，你都需要定一个 TypeBuilder。从现在开始，要研究 IL 实际生成的东西了，然后生成相同的内容。

```c#
public TypeBuilder GetType(ModuleBuilder moduleBuilder, string className)
{
    TypeBuilder typeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public);
    return typeBuilder;
}

private TypeBuilder GetType(ModuleBuilder moduleBuilder, string className, params string[] genericParameters) {
    TypeBuilder builder = moduleBuilder.DefineType(className, TypeAttributes.Public);
    var genericParametersBuilder = builder.DefineGenericParameters(genericParameters);
    foreach (var genBuilder in genericParametersBuilder)//We take each generic type T : class, new()
    {
        genBuilder.SetGenericParameterAttributes(GenericParameterAttributes.ReferenceTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint);
    }
    return builder;
}
```

这里有两个重载，第一个很简单，就是制定一个类名生成 TypeBuilder 返回即可。

第二个重载添加了一个额外的字符串参数数组，它定义了类的每个泛型类型。GenericTypeParameterBuilder 定义 GenericTypeParameter。定义并设置 GenericTypeParameters 的约束属性之后，你就可以生成它了。

### 4：创建方法

