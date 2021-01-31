using SSCMobileServiceBus.OfflineSync.Models.Exceptions;
using SSCMobileServiceBus.OfflineSync.Models.ThinViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SSCMobileServiceBus.OfflineSync.Models
{
    class ModelMapper
    {
    }
    public class BaseMapper
    {
        public TEntity MapViewModelToModel<TEntity>(object viewModel) where TEntity : class, new()
        {
            return ToModel<TEntity>(viewModel);
        }

        /// <summary>
        /// Creates a Model from the ViewModel
        /// </summary>
        /// <typeparam name="ModelType"></typeparam>
        /// <typeparam name="ViewModelType"></typeparam>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public ModelType ToModel<ModelType>(object viewModel) where ModelType : new()
        {

            ModelType model = new ModelType();

            try
            {
                var props = DehydrateObject<ModelPropertyAttribute>(viewModel);  // Find all attributes with ModelPropertyAttribute
                foreach (PropertyInfo info in props)
                {
                    // Find PropertyName on ViewModel, currentVaalue in ViewModel, and the CurrrentValue in the Model
                    var targetPropertyName = info.GetCustomAttribute(typeof(ModelPropertyAttribute)) as ModelPropertyAttribute;
                    var currentVal = info.GetValue(viewModel);
                    var targetVal = model.GetType().GetProperty(targetPropertyName.PropertyName).GetValue(model);

                    //if (targetPropertyName.PropertyName == "CaseAction") Debugger.Break();
                    if (currentVal != targetVal)
                    {
                        if (info.PropertyType.IsEnum)
                        {
                            var prop = model.GetType().GetProperty(info.Name);
                            //System.Type enumUnderlyingType = System.Enum.GetUnderlyingType(info);
                            //int caseAct = (int)targetVal.ToString();
                            prop.SetValue(model, Convert.ToInt32(currentVal));
                        }
                        else if (info.PropertyType.Name.Contains("ObservableCollection") || info.PropertyType.Name.Contains("ObservableRangeCollection"))
                        {
                            PropertyInfo modelProp = model.GetType().GetProperty(targetPropertyName.PropertyName);
                            var genericArg = modelProp.PropertyType.GenericTypeArguments.FirstOrDefault(); ;

                            if (targetVal == null)
                            {
                                var listType = typeof(List<>);
                                var constructedListType = listType.MakeGenericType(genericArg);
                                IList tempList = (IList)Activator.CreateInstance(constructedListType);
                                var list = GetThinListForModelCollection(currentVal as IEnumerable, genericArg);
                                var addMethod = modelProp.PropertyType.GetMethod("Add");
                                object instanceValue = modelProp.GetValue(model);
                                if (instanceValue == null)
                                {
                                    var tempListType = typeof(List<>);
                                    // var constructedListType = listType.MakeGenericType(modelProp.);
                                    instanceValue = Activator.CreateInstance(constructedListType);
                                    modelProp.SetValue(model, instanceValue);
                                }
                                foreach (var item in list)
                                {
                                    addMethod.Invoke(instanceValue, new object[] { item });
                                    //tempList.Add(item);
                                }

                                //var childd = model.GetType().GetProperty("Children");
                                //var addChild = modelProp.PropertyType.GetMethod("Add");
                                //modelProp.SetValue(model, tempList);
                                //targetVal = model.GetType().GetProperty(targetPropertyName.PropertyName).GetValue(model);
                            }

                            // fix for model
                            //    var list = GetThinListForCollection(currentVal as IEnumerable, genericArg);
                            //PropertyInfo modelListProp = model.GetType().GetProperty(targetPropertyName.PropertyName);
                            //modelListProp.PropertyType.GetMethod("ReplaceRange").Invoke(targetVal, new[] { list });

                        }
                        else
                        {
                            PropertyInfo modelProp = model.GetType().GetProperty(targetPropertyName.PropertyName);
                            modelProp.SetValue(model, ChangeType(currentVal, modelProp.PropertyType));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in ToModel: ex: " + ex);
            }
            return model;
        }

        /// <summary>
        /// Create a ThinViewModel for the Model collection
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public IEnumerable GetThinListForModelCollection(System.Collections.IEnumerable source, Type targetType)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(targetType);
            IList list = (IList)Activator.CreateInstance(constructedListType);
            foreach (var item in source)
            {
                var newObject = Activator.CreateInstance(targetType);

                PopulateModel(item, newObject);

                list.Add(newObject);
            }
            return list;
        }
        /// <summary>
        /// Conversion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T ChangeType<T>(object value)
        {
            var t = typeof(T);

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return default(T);
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return (T)Convert.ChangeType(value, t);
        }
        /// <summary>
        /// Conversion
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversion"></param>
        /// <returns></returns>
        public object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }
        /// <summary>
        /// Upate the view model from a model
        /// </summary>
        /// <param name="parentObject"></param>
        /// <param name="currentModel"></param>
        public void PopulateModel(object parentObject, object currentModel)
        {
            var props = DehydrateObject<ModelPropertyAttribute>(parentObject);
            foreach (PropertyInfo info in props)
            {
                var targetPropertyName = info.GetCustomAttribute(typeof(ModelPropertyAttribute)) as ModelPropertyAttribute;

                // if (info.Name == "Notes") Debugger.Break();// use this to debug a collection
                try
                {
                    var currentVal = info.GetValue(parentObject);
                    var targetVal = currentModel.GetType().GetProperty(targetPropertyName.PropertyName).GetValue(currentModel);

                    if (currentVal != targetVal)
                    {
                        // if it's an observable collection
                        if (info.PropertyType.Name.Contains("ObservableCollection") || info.PropertyType.Name.Contains("ObservableRangeCollection"))
                        {
                            var genericArg = info.PropertyType.GenericTypeArguments.FirstOrDefault(); ;

                            var list = GetThinListForCollection(targetVal as IEnumerable, genericArg);
                            info.PropertyType.GetMethod("ReplaceRange").Invoke(currentVal, new[] { list });

                        }
                        else // set object normally
                        {
                            var newProp = currentModel.GetType().GetProperty(targetPropertyName.PropertyName);
                            newProp.SetValue(currentModel, currentVal);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ConvertFromModelToViewModelException(ex.Message, info.Name, targetPropertyName.PropertyName, null);
                }
            }
        }
        public TViewModel MapToViewModel<TViewModel>(object model) where TViewModel : new()
        {

            //TEntity list = (TEntity)Activator.CreateInstance(typeof(TEntity));

            TViewModel viewModel = new TViewModel();

            PopulateViewModel(viewModel, model);

            return viewModel;


        }

        /// <summary>
        /// Get all properties of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<PropertyInfo> DehydrateObject<T>(object obj)
        {
            List<PropertyInfo> values =
                (from property in obj.GetType().GetProperties()
                 where property.GetCustomAttributes(typeof(T), false).Length > 0
                 select property).ToList();

            return values;
        }

        /// <summary>
        /// Upate the view model from a model
        /// </summary>
        /// <param name="parentObject"></param>
        /// <param name="currentModel"></param>
        public void PopulateViewModel(object parentObject, object currentModel)
        {
            var props = DehydrateObject<ModelPropertyAttribute>(parentObject);
            foreach (PropertyInfo info in props)
            {
                var targetPropertyName = info.GetCustomAttribute(typeof(ModelPropertyAttribute)) as ModelPropertyAttribute;

                // if (info.Name == "Notes") Debugger.Break();// use this to debug a collection
                try
                {
                    var currentVal = info.GetValue(parentObject);
                    var a1 = currentModel.GetType();
                    var a2 = currentModel.GetType().GetProperty(targetPropertyName.PropertyName);


                    var targetVal = currentModel.GetType().GetProperty(targetPropertyName.PropertyName).GetValue(currentModel);

                    if (currentVal != targetVal)
                    {

                        // if it's an observable collection
                        if (info.PropertyType.Name.Contains("ObservableCollection") || info.PropertyType.Name.Contains("ObservableRangeCollection"))
                        {
                            var genericArg = info.PropertyType.GenericTypeArguments.FirstOrDefault(); ;

                            var list = GetThinListForCollection(targetVal as IEnumerable, genericArg);
                            info.PropertyType.GetMethod("ReplaceRange").Invoke(currentVal, new[] { list });

                        }
                        else if (info.PropertyType.IsSubclassOf(typeof(BaseThinViewModel)))
                        {
                            // if type is a thin model
                            var item = GetThinViewModel(info.PropertyType, targetVal);
                            info.SetValue(parentObject, item);
                        }
                        else // set object normally
                        {
                            info.SetValue(parentObject, targetVal);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ConvertFromModelToViewModelException(ex.Message, info.Name, targetPropertyName.PropertyName, null);
                }
            }
        }

        /// <summary>
        /// Create a ThinViewModel for the Model collection
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public IEnumerable GetThinListForCollection(System.Collections.IEnumerable source, Type targetType)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(targetType);
            IList list = (IList)Activator.CreateInstance(constructedListType);
            if (source != null)
            {
                foreach (var item in source)
                {
                    var newObject = Activator.CreateInstance(targetType);

                    PopulateViewModel(newObject, item);

                    list.Add(newObject);
                }
            }

            return list;
        }
        public BaseThinViewModel GetThinViewModel(Type targetType, object model)
        {
            //try
            //{
            BaseThinViewModel newObject = Activator.CreateInstance(targetType) as BaseThinViewModel;


            PopulateViewModel(newObject, model);
            return newObject;
            //} catch ( Exception ex)
            //{

            //}
        }
    }
}
