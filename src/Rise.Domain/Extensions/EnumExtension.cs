using System.Reflection;
using System.ComponentModel;
using Rise.Domain.Entities;
namespace Rise.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this FoodCategory value)
        {
            var member = value.GetType().GetMember(value.ToString())[0];
            return member.GetCustomAttribute<DescriptionAttribute>()?.Description ?? value.ToString();
        }
    }
}