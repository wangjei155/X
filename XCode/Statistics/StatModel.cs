﻿using System;
using System.Collections.Generic;
using NewLife.Reflection;

namespace XCode.Statistics
{
    /// <summary>统计模型</summary>
    /// <typeparam name="T"></typeparam>
    public class StatModel<T> : StatModel where T : StatModel<T>, new()
    {
        #region 方法
        /// <summary>拷贝</summary>
        /// <param name="model"></param>
        public virtual void Copy(T model)
        {
            Time = model.Time;
            Level = model.Level;
        }

        /// <summary>克隆到目标类型</summary>
        /// <returns></returns>
        public virtual T Clone()
        {
            var model = new T();
            model.Copy(this);

            return model;
        }

        /// <summary>分割为多个层级</summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        public virtual List<T> Split(params StatLevels[] levels)
        {
            var list = new List<T>();
            foreach (var item in levels)
            {
                var st = Clone();
                st.Level = item;
                st.Time = st.GetDate(item);

                list.Add(st);
            }

            return list;
        }
        #endregion
    }

    /// <summary>统计模型</summary>
    public class StatModel
    {
        #region 属性
        /// <summary>时间</summary>
        public DateTime Time { get; set; }

        /// <summary>层级</summary>
        public StatLevels Level { get; set; }
        #endregion

        #region 构造
        /// <summary>实例化</summary>
        public StatModel() { }
        #endregion

        #region 方法
        /// <summary>获取不同层级的时间。选择层级区间的开头</summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public DateTime GetDate(StatLevels level)
        {
            var dt = Time;
            switch (level)
            {
                case StatLevels.All: return new DateTime(1, 1, 1);
                case StatLevels.Year: return new DateTime(dt.Year, 1, 1);
                case StatLevels.Month: return new DateTime(dt.Year, dt.Month, 1);
                case StatLevels.Day: return dt.Date;
                case StatLevels.Hour: return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                case StatLevels.Minute: return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
                default:
                    break;
            }

            return dt;
        }

        /// <summary>数据库时间转显示字符串</summary>
        /// <returns></returns>
        public override String ToString()
        {
            var dt = Time;
            switch (Level)
            {
                case StatLevels.All: return "全部";
                case StatLevels.Year: return "{0:yyyy}".F(dt);
                case StatLevels.Month: return "{0:yyyy-MM}".F(dt);
                case StatLevels.Day: return "{0:yyyy-MM-dd}".F(dt);
                case StatLevels.Hour: return "{0:yyyy-MM-dd HH}".F(dt);
                case StatLevels.Minute: return "{0:yyyy-MM-dd HH:mm}".F(dt);
                default: return "全部";
            }
        }

        /// <summary>使用参数填充</summary>
        /// <param name="ps"></param>
        public virtual void Fill(IDictionary<String, String> ps)
        {
            //this.Copy(ps, true);

            foreach (var pi in GetType().GetProperties())
            {
                if (!pi.CanWrite) continue;
                if (pi.GetIndexParameters().Length > 0) continue;

                if (ps.TryGetValue(pi.Name, out var val)) this.SetValue(pi, val.ChangeType(pi.PropertyType));
            }
        }
        #endregion
    }
}