using System;
using Abp.Json;
using Shouldly;
using Xunit;

namespace MyCompanyName.AbpZeroTemplate.Tests.Json
{
    public class TimeSpanToJsonStringConverter_Tests
    {
        public class TimeSpanModel
        {
            public TimeSpan MyTimeSpan { get; set; }

            public TimeSpan? MyTimeSpan2 { get; set; }

            public TimeSpan? MyNullableTimeSpan { get; set; }

            public TimeSpan? MyNullableTimeSpan2 { get; set; }
        }

        [Fact]
        public void TimeSpanToJsonStringConverter_Test()
        {
            var obj = new TimeSpanModel
            {
                MyTimeSpan = TimeSpan.FromMinutes(72),
                MyTimeSpan2 = null,
                MyNullableTimeSpan = null,
                MyNullableTimeSpan2 = TimeSpan.FromMinutes(72)
            };
            var jsonString = obj.ToJsonString();

            //WriteJson assert
            jsonString.ShouldBe("{\"MyTimeSpan\":\"01:12:00\",\"MyTimeSpan2\":null,\"MyNullableTimeSpan\":null,\"MyNullableTimeSpan2\":\"01:12:00\"}");

            var obj2 = jsonString.FromJsonString<TimeSpanModel>();

            //ReadJson assert
            obj2.MyTimeSpan.ShouldBe(TimeSpan.FromMinutes(72));
            obj2.MyTimeSpan2.ShouldBeNull();
            obj2.MyNullableTimeSpan.ShouldBeNull();
            obj2.MyNullableTimeSpan2.ShouldBe(TimeSpan.FromMinutes(72));
        }
    }
}
