alter table `BlogComment`  drop foreign key FK7E99D6A4D81B04A5;


    
alter table `BlogComment`  drop foreign key FK7E99D6A444DCE7EC;


    
alter table `EntryObject`  drop foreign key FK972D1BCCF8885880;


    
alter table `EntryObject`  drop foreign key FK972D1BCCD5E0FE61;


    
alter table `A6WEntry`  drop foreign key FKF8CB6856433FDD88;


    
alter table `BlogEntry`  drop foreign key FKE1971E24433FDD88;


    
alter table `SizeEntry`  drop foreign key FKB5645F28433FDD88;


    
alter table `SizeEntry`  drop foreign key FKB5645F285FD0E9B1;


    
alter table `StrengthTrainingEntry`  drop foreign key FKCE3308D5433FDD88;


    
alter table `SuplementsEntry`  drop foreign key FK49C2085B433FDD88;


    
alter table `Exercise`  drop foreign key FKE31A2352D81B04A5;


    
alter table `FriendInvitation`  drop foreign key FKDA11F418707176BE;


    
alter table `FriendInvitation`  drop foreign key FKDA11F4188037A598;


    
alter table `Message`  drop foreign key FKC1F33ACC30CAA4FF;


    
alter table `Message`  drop foreign key FKC1F33ACCEA3533DF;


    
alter table `MyTraining`  drop foreign key FKE60DDB64D81B04A5;


    
alter table `Profile`  drop foreign key FK625D1A0B5FD0E9B1;


    
alter table TrainingPlanToProfile  drop foreign key FK444DB695AB82EBE1;


    
alter table TrainingPlanToProfile  drop foreign key FK444DB695D81B04A5;


    
alter table FriendsForProfile  drop foreign key FK80FA58323B5755DB;


    
alter table FriendsForProfile  drop foreign key FK80FA5832285E791C;


    
alter table FavoriteUsersToFavoriteUsers  drop foreign key FK23CCC3703B5755DB;


    
alter table FavoriteUsersToFavoriteUsers  drop foreign key FK23CCC370285E791C;


    
alter table `Serie`  drop foreign key FK4799D818AC508320;


    
alter table `StrengthTrainingItem`  drop foreign key FK5F3E84862D29B34C;


    
alter table `SuplementItem`  drop foreign key FK345E5E5A2A287A2C;


    
alter table `TrainingDay`  drop foreign key FKC18A9A4FD81B04A5;


    
alter table `TrainingPlan`  drop foreign key FK674AABBBD81B04A5;


    drop table if exists `BlogComment`;

    drop table if exists `EntryObject`;

    drop table if exists `A6WEntry`;

    drop table if exists `BlogEntry`;

    drop table if exists `SizeEntry`;

    drop table if exists `StrengthTrainingEntry`;

    drop table if exists `SuplementsEntry`;

    drop table if exists `Exercise`;

    drop table if exists `FriendInvitation`;

    drop table if exists `LoginData`;

    drop table if exists `Message`;

    drop table if exists `MyTraining`;

    drop table if exists `Profile`;

    drop table if exists TrainingPlanToProfile;

    drop table if exists FriendsForProfile;

    drop table if exists FavoriteUsersToFavoriteUsers;

    drop table if exists `RatingUserValue`;

    drop table if exists `Serie`;

    drop table if exists `StrengthTrainingItem`;

    drop table if exists `SuplementItem`;

    drop table if exists `Suplement`;

    drop table if exists `TrainingDay`;

    drop table if exists `TrainingPlan`;

    drop table if exists `Wymiary`;

    create table `BlogComment` (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       Comment longtext not null,
       DateTime DATETIME not null,
       CommentType INTEGER not null,
       Profile_id INTEGER not null,
       BlogEntry_id INTEGER not null,
       primary key (Id)
    )ENGINE = 'InnoDB';

    create table `EntryObject` (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       Comment longtext,
       Name VARCHAR(100),
       ReportStatus INTEGER not null,
       TrainingDay_id INTEGER,
       MyTraining_id INTEGER,
       primary key (Id)
    )ENGINE = 'InnoDB';

    create table `A6WEntry` (
        EntryObject_id INTEGER not null,
       Completed TINYINT(1) not null,
       DayNumber INTEGER not null,
       Set1 INTEGER,
       Set2 INTEGER,
       Set3 INTEGER,
       primary key (EntryObject_id)
    )ENGINE = 'InnoDB';

    create table `BlogEntry` (
        EntryObject_id INTEGER not null,
       LastCommentDate DATETIME,
       AllowComments TINYINT(1) not null,
       primary key (EntryObject_id)
    )ENGINE = 'InnoDB';

    create table `SizeEntry` (
        EntryObject_id INTEGER not null,
       Wymiary_id INTEGER,
       primary key (EntryObject_id)
    )ENGINE = 'InnoDB';

    create table `StrengthTrainingEntry` (
        EntryObject_id INTEGER not null,
       EndTime DATETIME,
       StartTime DATETIME,
       TrainingPlanItemId char(36),
       TrainingPlanId char(36),
       Intensity INTEGER not null,
       primary key (EntryObject_id)
    )ENGINE = 'InnoDB';

    create table `SuplementsEntry` (
        EntryObject_id INTEGER not null,
       primary key (EntryObject_id)
    )ENGINE = 'InnoDB';

    create table `Exercise` (
        GlobalId char(36) not null,
       Version INTEGER not null,
       Name VARCHAR(100) not null,
       Description longtext,
       Url TEXT,
       Shortcut VARCHAR(20) not null,
       PublishDate DATETIME,
       Status INTEGER not null,
       MechanicsType INTEGER not null,
       ExerciseForceType INTEGER not null,
       ExerciseType INTEGER not null,
       Difficult INTEGER not null,
       Profile_id INTEGER,
       primary key (GlobalId)
    )ENGINE = 'InnoDB';

    create table `FriendInvitation` (
        Inviter INTEGER not null,
       Invited INTEGER not null,
       CreateDate DATETIME not null,
       InvitationType VARCHAR(255) not null,
       Message TEXT,
       primary key (Inviter, Invited)
    )ENGINE = 'InnoDB';

    create table `LoginData` (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       ApplicationLanguage VARCHAR(3) not null,
       ApplicationVersion VARCHAR(10) not null,
       ClientInstanceId char(36) not null,
       ProfileId INTEGER not null,
       LoginDateTime DATETIME not null,
       Platform INTEGER not null,
       PlatformVersion VARCHAR(100),
       primary key (Id)
    )ENGINE = 'InnoDB';

    create table `Message` (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       MessageType INTEGER not null,
       Topic VARCHAR(100),
       Priority INTEGER not null,
       ContentType INTEGER not null,
       Content TEXT,
       CreatedDate DATETIME not null,
       Sender_id INTEGER,
       Receiver_id INTEGER,
       primary key (Id)
    )ENGINE = 'InnoDB';

    create table `MyTraining` (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       StartDate DATETIME not null,
       EndDate DATETIME,
       TypeId char(36) not null,
       TrainingEnd INTEGER,
       PercentageCompleted INTEGER,
       Name VARCHAR(100) not null,
       Profile_id INTEGER not null,
       primary key (Id)
    )ENGINE = 'InnoDB';

    create table `Profile` (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       Version INTEGER not null,
       PreviousClientInstanceId char(36),
       ActivationId VARCHAR(40),
       UserName VARCHAR(100) not null unique,
       Password VARCHAR(100),
       Gender INTEGER,
       Birthday DATETIME not null,
       CountryId INTEGER not null,
       AboutInformation longtext,
       Email VARCHAR(255) not null unique,
       IsDeleted TINYINT(1) not null,
       CreationDate DATETIME not null,
       Wymiary_id INTEGER,
       PictureId char(36),
       Hash VARCHAR(255),
       CalendarView INTEGER not null,
       Sizes INTEGER not null,
       Friends INTEGER not null,
       BirthdayDate INTEGER not null,
       Searchable TINYINT(1) not null,
       primary key (Id)
    )ENGINE = 'InnoDB';

    create table TrainingPlanToProfile (
        Profile_id INTEGER not null,
       TrainingPlan_id char(36) not null
    )ENGINE = 'InnoDB';

    create table FriendsForProfile (
        parent_profile_id INTEGER not null,
       child_profile_id INTEGER not null
    )ENGINE = 'InnoDB';

    create table FavoriteUsersToFavoriteUsers (
        parent_profile_id INTEGER not null,
       child_profile_id INTEGER not null
    )ENGINE = 'InnoDB';

    create table `RatingUserValue` (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       ProfileId INTEGER not null,
       RatedObjectId char(36) not null,
       Rating FLOAT not null,
       VotedDate DATETIME not null,
       ShortComment TEXT,
       primary key (Id)
    )ENGINE = 'InnoDB';

    create table `Serie` (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       IsCiezarBezSztangi TINYINT(1) not null,
       SetType INTEGER not null,
       RepetitionNumber INTEGER,
       Weight FLOAT,
       DropSet INTEGER not null,
       TrainingPlanItemId char(36),
       StartTime DATETIME,
       EndTime DATETIME,
       Comment longtext,
       StrengthTrainingItem_id INTEGER,
       primary key (Id)
    )ENGINE = 'InnoDB';

    create table `StrengthTrainingItem` (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       ExerciseId char(36) not null,
       Position INTEGER not null,
       Comment longtext,
       TrainingPlanItemId char(36),
       SuperSetGroup VARCHAR(30),
       StrengthTrainingEntry_id INTEGER,
       primary key (Id)
    )ENGINE = 'InnoDB';

    create table `SuplementItem` (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       DosageType INTEGER not null,
       Dosage DOUBLE not null,
       SuplementId char(36) not null,
       Time DATETIME not null,
       Name VARCHAR(100),
       Comment longtext,
       SuplementsEntry_id INTEGER,
       primary key (Id)
    )ENGINE = 'InnoDB';

    create table `Suplement` (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       Name VARCHAR(100) not null,
       SuplementId char(36) not null unique,
       ProfileId INTEGER,
       Comment longtext,
       Url TEXT,
       primary key (Id)
    )ENGINE = 'InnoDB';

    create table `TrainingDay` (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       Version INTEGER not null,
       TrainingDate DATETIME not null,
       Comment longtext,
       Profile_id INTEGER not null,
       primary key (Id)
    )ENGINE = 'InnoDB';

    create table `TrainingPlan` (
        GlobalId char(36) not null,
       Version INTEGER not null,
       Name VARCHAR(100) not null,
       Author VARCHAR(100) not null,
       CreationDate DATETIME not null,
       DaysCount INTEGER not null,
       Language VARCHAR(255) not null,
       PublishDate DATETIME,
       TrainingType INTEGER not null,
       Purpose INTEGER not null,
       Status INTEGER not null,
       Difficult INTEGER not null,
       PlanContent longtext not null,
       Profile_id INTEGER not null,
       primary key (GlobalId)
    )ENGINE = 'InnoDB';

    create table `Wymiary` (
        Id INTEGER NOT NULL AUTO_INCREMENT,
       DateTime DATETIME not null,
       IsNaCzczo TINYINT(1),
       Weight FLOAT not null,
       Klatka FLOAT not null,
       RightBiceps FLOAT not null,
       LeftBiceps FLOAT not null,
       Pas FLOAT not null,
       RightForearm FLOAT not null,
       LeftForearm FLOAT not null,
       RightUdo FLOAT not null,
       LeftUdo FLOAT not null,
       Height INTEGER not null,
       primary key (Id)
    )ENGINE = 'InnoDB';

    alter table `BlogComment` 
        add index (Profile_id), 
        add constraint FK7E99D6A4D81B04A5 
        foreign key (Profile_id) 
        references `Profile` (Id);

    alter table `BlogComment` 
        add index (BlogEntry_id), 
        add constraint FK7E99D6A444DCE7EC 
        foreign key (BlogEntry_id) 
        references `BlogEntry` (EntryObject_id);

    alter table `EntryObject` 
        add index (TrainingDay_id), 
        add constraint FK972D1BCCF8885880 
        foreign key (TrainingDay_id) 
        references `TrainingDay` (Id);

    alter table `EntryObject` 
        add index (MyTraining_id), 
        add constraint FK972D1BCCD5E0FE61 
        foreign key (MyTraining_id) 
        references `MyTraining` (Id);

    alter table `A6WEntry` 
        add index (EntryObject_id), 
        add constraint FKF8CB6856433FDD88 
        foreign key (EntryObject_id) 
        references `EntryObject` (Id);

    alter table `BlogEntry` 
        add index (EntryObject_id), 
        add constraint FKE1971E24433FDD88 
        foreign key (EntryObject_id) 
        references `EntryObject` (Id);

    alter table `SizeEntry` 
        add index (EntryObject_id), 
        add constraint FKB5645F28433FDD88 
        foreign key (EntryObject_id) 
        references `EntryObject` (Id);

    alter table `SizeEntry` 
        add index (Wymiary_id), 
        add constraint FKB5645F285FD0E9B1 
        foreign key (Wymiary_id) 
        references `Wymiary` (Id);

    alter table `StrengthTrainingEntry` 
        add index (EntryObject_id), 
        add constraint FKCE3308D5433FDD88 
        foreign key (EntryObject_id) 
        references `EntryObject` (Id);

    alter table `SuplementsEntry` 
        add index (EntryObject_id), 
        add constraint FK49C2085B433FDD88 
        foreign key (EntryObject_id) 
        references `EntryObject` (Id);

    alter table `Exercise` 
        add index (Profile_id), 
        add constraint FKE31A2352D81B04A5 
        foreign key (Profile_id) 
        references `Profile` (Id);

    alter table `FriendInvitation` 
        add index (Inviter), 
        add constraint FKDA11F418707176BE 
        foreign key (Inviter) 
        references `Profile` (Id);

    alter table `FriendInvitation` 
        add index (Invited), 
        add constraint FKDA11F4188037A598 
        foreign key (Invited) 
        references `Profile` (Id);

    alter table `Message` 
        add index (Sender_id), 
        add constraint FKC1F33ACC30CAA4FF 
        foreign key (Sender_id) 
        references `Profile` (Id);

    alter table `Message` 
        add index (Receiver_id), 
        add constraint FKC1F33ACCEA3533DF 
        foreign key (Receiver_id) 
        references `Profile` (Id);

    alter table `MyTraining` 
        add index (Profile_id), 
        add constraint FKE60DDB64D81B04A5 
        foreign key (Profile_id) 
        references `Profile` (Id);

    alter table `Profile` 
        add index (Wymiary_id), 
        add constraint FK625D1A0B5FD0E9B1 
        foreign key (Wymiary_id) 
        references `Wymiary` (Id);

    alter table TrainingPlanToProfile 
        add index (TrainingPlan_id), 
        add constraint FK444DB695AB82EBE1 
        foreign key (TrainingPlan_id) 
        references `TrainingPlan` (GlobalId);

    alter table TrainingPlanToProfile 
        add index (Profile_id), 
        add constraint FK444DB695D81B04A5 
        foreign key (Profile_id) 
        references `Profile` (Id);

    alter table FriendsForProfile 
        add index (child_profile_id), 
        add constraint FK80FA58323B5755DB 
        foreign key (child_profile_id) 
        references `Profile` (Id);

    alter table FriendsForProfile 
        add index (parent_profile_id), 
        add constraint FK80FA5832285E791C 
        foreign key (parent_profile_id) 
        references `Profile` (Id);

    alter table FavoriteUsersToFavoriteUsers 
        add index (child_profile_id), 
        add constraint FK23CCC3703B5755DB 
        foreign key (child_profile_id) 
        references `Profile` (Id);

    alter table FavoriteUsersToFavoriteUsers 
        add index (parent_profile_id), 
        add constraint FK23CCC370285E791C 
        foreign key (parent_profile_id) 
        references `Profile` (Id);

    alter table `Serie` 
        add index (StrengthTrainingItem_id), 
        add constraint FK4799D818AC508320 
        foreign key (StrengthTrainingItem_id) 
        references `StrengthTrainingItem` (Id);

    alter table `StrengthTrainingItem` 
        add index (StrengthTrainingEntry_id), 
        add constraint FK5F3E84862D29B34C 
        foreign key (StrengthTrainingEntry_id) 
        references `StrengthTrainingEntry` (EntryObject_id);

    alter table `SuplementItem` 
        add index (SuplementsEntry_id), 
        add constraint FK345E5E5A2A287A2C 
        foreign key (SuplementsEntry_id) 
        references `SuplementsEntry` (EntryObject_id);

    alter table `TrainingDay` 
        add index (Profile_id), 
        add constraint FKC18A9A4FD81B04A5 
        foreign key (Profile_id) 
        references `Profile` (Id);

    alter table `TrainingPlan` 
        add index (Profile_id), 
        add constraint FK674AABBBD81B04A5 
        foreign key (Profile_id) 
        references `Profile` (Id);