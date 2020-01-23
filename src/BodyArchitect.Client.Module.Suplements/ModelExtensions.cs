using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.Suplements
{
    public static class ModelExtensions
    {
        public static string TranslateSupple(this string key)
        {
            return EnumLocalizer.Default.GetUIString(
                    "BodyArchitect.Client.Module.Suplements:SuplementsEntryStrings:" + key);
        }
        public static bool IsFavorite(this SupplementCycleDefinitionDTO definition)
        {
            SupplementsCycleDefinitionsReposidory.Instance.EnsureLoaded();
            var res = (from e in SupplementsCycleDefinitionsReposidory.Instance.Items.Values where e.GlobalId == definition.GlobalId && e.Profile != null && !e.IsMine() select e).Count();
            return res > 0;
        }

        public static bool CanAddToFavorites(this SupplementCycleDefinitionDTO definition)
        {
            return !(definition == null || definition.Profile == null || definition.IsFavorite() || definition.IsMine());
        }

        public static bool CanRemoveFromFavorites(this SupplementCycleDefinitionDTO definition)
        {
            return !(definition == null || definition.Profile == null || !definition.IsFavorite() || definition.IsMine());
        }

        public static bool AddToFavorites(this SupplementCycleDefinitionDTO definition)
        {
            if (!CanAddToFavorites(definition) )
            {
                return false;
            }
            ServiceCommand command = new ServiceCommand(() =>
            {
                try
                {
                    var param = new SupplementsCycleDefinitionOperationParam();
                    param.SupplementsCycleDefinitionId = definition.GlobalId;
                    param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                    ServiceManager.SupplementsCycleDefinitionOperation(param);
                }
                catch (ObjectIsFavoriteException)
                {
                }
                catch
                {
                    SupplementsCycleDefinitionsReposidory.Instance.Remove(definition.GlobalId);
                    throw;
                }
            });
            ServicePool.Add(command);
            SupplementsCycleDefinitionsReposidory.Instance.Add(definition);
            return true;
        }

        public static bool RemoveFromFavorites(this SupplementCycleDefinitionDTO definition)
        {
            if (!CanRemoveFromFavorites(definition))
            {
                return false;
            }
            //ServiceManager.WorkoutPlanFavoritesOperation(plan, FavoriteOperation.Remove);

            ServiceCommand command = new ServiceCommand(() =>
            {
                try
                {
                    var param = new SupplementsCycleDefinitionOperationParam();
                    param.SupplementsCycleDefinitionId = definition.GlobalId;
                    param.Operation = SupplementsCycleDefinitionOperation.RemoveFromFavorites;
                    ServiceManager.SupplementsCycleDefinitionOperation(param);
                }
                catch (ObjectIsNotFavoriteException)
                {

                }
                catch
                {
                    SupplementsCycleDefinitionsReposidory.Instance.Add(definition);
                    throw;
                }

            });
            ServicePool.Add(command);
            SupplementsCycleDefinitionsReposidory.Instance.Remove(definition.GlobalId);
            return true;
        }
    }
}
