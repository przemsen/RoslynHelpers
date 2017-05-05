# RoslynHelpers #
Simplistic command line tools based on Roslyn APIs to help with some tedious development activities

## ModelHelper ##
Generates JSON model from C# class with typing information in comments. Useful when testing REST APIs.

```
modelhelper solution_name.sln project_name class_name_with_namespace
{
Date: null,                              //  DateTime? from TypeX
Id: null,                                //          T from TypeY
}
```

## CtorHelper ##
Generates constructor call with type information for all of its arguments. Useful when writing tests for classes with too big constructor argument lists

```
ctorhelper solution_name.sln project_name class_name_with_namespace
new ClassName(
  null, // IInterface1
  null, // IInterface2
);
```

