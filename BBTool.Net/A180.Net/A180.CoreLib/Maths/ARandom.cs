﻿namespace A180.CoreLib.Maths;

public static class ARandom<T>
{
    public class RandomConfig
    {
        public RandomConfig(T v, int w)
        {
            Value = v;
            Weight = w;
        }

        public T Value { get; set; }

        public int Weight { get; set; }
    }

    public static RandomConfig Generate(List<RandomConfig> configList)
    {
        //累加结算总权重
        int totalWeight = configList.Aggregate(0, (all, next) => all += next.Weight);

        //在0~total范围内随机
        int cursor = 0;
        int random = new Random().Next(0, totalWeight);
        foreach (var item in configList)
        {
            //累加当前权重
            cursor += item.Weight;
            //判断随机数
            if (cursor > random)
            {
                return item;
            }
        }

        throw new Exception("不可能执行到的代码被触发");
    }
}