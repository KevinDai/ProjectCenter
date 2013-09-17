using EmitMapper;
using EmitMapper.MappingConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCenter.Web.Extensions
{
    public static class MappingExtensions
    {

        public static T Map<T>(this object source)
        {
            if (source == null)
            {
                return default(T);
            }

            return (T)ObjectMapperManager.DefaultInstance
                .GetMapperImpl(source.GetType(), typeof(T), DefaultMapConfig.Instance)
                .Map(source);
        }

        public static T Map<T>(this object source, T target)
        {
            if (source == null)
            {
                return default(T);
            }

            return (T)ObjectMapperManager.DefaultInstance
                .GetMapperImpl(source.GetType(), typeof(T), DefaultMapConfig.Instance)
                .Map(source, target, null);
        }
    }
}