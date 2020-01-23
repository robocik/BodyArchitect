using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BodyArchitect.Service.V2.Model
{
    public partial class GetRemindersParam
    {
        public GetRemindersParam()
        {
            Types=new List<ReminderType>();
        }
    }


    public partial class PagedResultOfCustomerDTO5oAtqRlh : IPagedResult<CustomerDTO>
    {
        public PagedResultOfCustomerDTO5oAtqRlh()
        {
            ItemsField = new List<CustomerDTO>();
        }
    }

    public partial class PagedResultOfTrainingPlan5oAtqRlh : IPagedResult<TrainingPlan>
    {
        public PagedResultOfTrainingPlan5oAtqRlh()
        {
            ItemsField = new List<TrainingPlan>();
        }
    }

    public partial class PagedResultOfMyPlaceDTO5oAtqRlh : IPagedResult<MyPlaceDTO>
    {
        public PagedResultOfMyPlaceDTO5oAtqRlh()
        {
            ItemsField = new List<MyPlaceDTO>();
        }
    }

    public partial class PagedResultOfCommentEntryDTO5oAtqRlh : IPagedResult<CommentEntryDTO>
    {
        public PagedResultOfCommentEntryDTO5oAtqRlh()
        {
            Items = new List<CommentEntryDTO>();
        }
    }

    public partial class PagedResultOfUserSearchDTO5oAtqRlh : IPagedResult<UserSearchDTO>
    {
        public PagedResultOfUserSearchDTO5oAtqRlh()
        {
            Items = new List<UserSearchDTO>();
        }
    }

    public partial class PagedResultOfReminderItemDTO5oAtqRlh:IPagedResult<ReminderItemDTO>
    {
        public PagedResultOfReminderItemDTO5oAtqRlh()
        {
            Items = new List<ReminderItemDTO>();
        }
    }

    public partial class PagedResultOfTrainingDayCommentDTO5oAtqRlh : IPagedResult<TrainingDayCommentDTO>
    {
        public PagedResultOfTrainingDayCommentDTO5oAtqRlh()
        {
            Items = new List<TrainingDayCommentDTO>();
        }
    }

    public partial class PagedResultOfTrainingDayDTO5oAtqRlh : IPagedResult<TrainingDayDTO>
    {
        public PagedResultOfTrainingDayDTO5oAtqRlh()
        {
            Items = new List<TrainingDayDTO>();
        }
    }

    public partial class ExerciseSearchCriteria
    {
        public ExerciseSearchCriteria()
        {
            ExerciseTypesField=new List<ExerciseType>();
            SearchGroupsField=new List<ExerciseSearchCriteriaGroup>();
        }
    }

    public partial class PagedResultOfExerciseDTO5oAtqRlh : IPagedResult<ExerciseDTO>
    {
        public PagedResultOfExerciseDTO5oAtqRlh()
        {
            Items = new List<ExerciseDTO>();
        }
    }

    public partial class PagedResultOfSuplementDTO5oAtqRlh : IPagedResult<SuplementDTO>
    {
        public PagedResultOfSuplementDTO5oAtqRlh()
        {
            Items = new List<SuplementDTO>();
        }
    }

    public partial class PagedResultOfMessageDTO5oAtqRlh:IPagedResult<MessageDTO>
    {
        public PagedResultOfMessageDTO5oAtqRlh()
        {
            Items = new List<MessageDTO>();
        }
    }

    public partial class GetFeaturedDataCompletedEventArgs : IServiceResult<FeaturedData>
    {
        public FeaturedData MyResult
        {
            get { return Result; }
        }
    }
    public partial class GetCustomersCompletedEventArgs : IServicePagedResult<CustomerDTO>
    {
        public IPagedResult<CustomerDTO> MyResult
        {
            get { return Result; }
        }
    }
    
    public partial class GetSuplementsCompletedEventArgs : IServicePagedResult<SuplementDTO>
    {
        public IPagedResult<SuplementDTO> MyResult
        {
            get { return Result; }
        }
    }

    public partial class GetExercisesCompletedEventArgs : IServicePagedResult<ExerciseDTO>
    {
        public IPagedResult<ExerciseDTO> MyResult
        {
            get { return Result; }
        }
    }

    public partial class GetRemindersCompletedEventArgs : IServicePagedResult<ReminderItemDTO>
    {
        public IPagedResult<ReminderItemDTO> MyResult
        {
            get { return Result; }
        }
    }

    public partial class GetWorkoutPlansCompletedEventArgs : IServicePagedResult<TrainingPlan>
    {
        public IPagedResult<TrainingPlan> MyResult
        {
            get { return Result; }
        }
    }

    public partial class GetMyPlacesCompletedEventArgs : IServicePagedResult<MyPlaceDTO>
    {
        public IPagedResult<MyPlaceDTO> MyResult
        {
            get { return Result; }
        }
    }

    public partial class GetMessagesCompletedEventArgs : IServicePagedResult<MessageDTO>
    {
        public IPagedResult<MessageDTO> MyResult
        {
            get { return Result; }
        }
    }
}
