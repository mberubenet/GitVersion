using System;

namespace GitVersion.AcceptanceTests
{
    using System.Runtime.CompilerServices;
    using Shouldly;
    using TechTalk.SpecFlow;

    public static class SpecFlowHelper
    {
        public static T GetOrCreate<T>(Func<T> createInstanceFunc, [CallerMemberName]string propertyName = "") where T : class
        {
            T currentProperty = Get<T>(propertyName);

            if (currentProperty == null)
            {
                currentProperty = createInstanceFunc();
                ScenarioContext.Current.Add(propertyName, currentProperty);
            }
            return currentProperty;
        }
        public static T Get<T>([CallerMemberName]string propertyName = "") where T : class
        {
            propertyName.ShouldNotBeNullOrWhiteSpace();
            if (ScenarioContext.Current.ContainsKey(propertyName))
            {
                return ScenarioContext.Current[propertyName] as T;
            }
            return default(T);
        }
        public static void Set<T>(T value, [CallerMemberName]string propertyName = "") where T : class
        {
            propertyName.ShouldNotBeNullOrWhiteSpace();
            if (ScenarioContext.Current.ContainsKey(propertyName))
            {
                ScenarioContext.Current[propertyName] = value;
            }
            ScenarioContext.Current.Add(propertyName, value);
        }
    }
}
