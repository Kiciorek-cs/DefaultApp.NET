using System;
using System.Linq;

namespace Demo.BLL.Validators;

public static class ValidationKeys
{
    public static readonly Guid LowerThen = new("dc2a442f-494b-49e2-a8cc-f25bf2df2638");
    public static readonly Guid GreaterThan = new("a04aaed8-4064-4f13-952c-8c17cacbbdb5");
    public static readonly Guid NotEmpty = new("48f7d7f2-8cf3-4997-a776-f8cf4e2221fc");
    public static readonly Guid NotNull = new("591b5f2f-87ad-4e94-b248-2f6171452e97");
    public static readonly Guid Equal = new("66a0e174-26aa-4969-b3d2-70ac257ecb72");
    public static readonly Guid NotEqual = new("0cc62893-ed97-4888-992d-cce8e842f538");

    public static readonly Guid NullIntTimeout = new("2c12550e-68aa-4dcc-85fd-ca25249c320d");
    public static readonly Guid EmailIsNotCorrect = new("d5496240-8125-4763-a9f1-26665acef7f9");
    public static readonly Guid IpAddressIsNotCorrect = new("bbd5c76c-7aa3-4fca-8e80-d8565a2b13b6");

    public static readonly Guid PasswordLowerChar = new("b26a771b-3bd9-4c33-a60c-04c813f67569");
    public static readonly Guid PasswordUpperChar = new("0028aed3-ce3a-4884-9e0a-829ac799c212");
    public static readonly Guid PasswordMiniMaxChars = new("4cad265f-9956-447c-a565-e0c73fd4b56e");
    public static readonly Guid PasswordNumber = new("29b92d04-10fd-4e77-8af4-5ed140e31099");
    public static readonly Guid PasswordSymbols = new("3c06ecfa-8abe-4152-aee0-c26d4587c5be");

    public static readonly Guid UserExist = new("7a8a6f6a-3f81-43ff-8be4-63b4a960c75d");
    public static readonly Guid UserNotExist = new("c2c99bce-fadb-4b30-aa53-83ebbf65cbcb");

    public static readonly Guid TokenIsNotValid = new("254f14e4-b533-4ef8-bbe1-ebc7e84712f1");

    public static readonly Guid UserConfirmed = new("516d9c4f-d3c9-4f77-ab64-dd81123686c1");

    public static readonly Guid UserSuspended = new("8c83f078-6929-46a7-ba62-a6de72554e91");
    public static readonly Guid UserDeleted = new("5c17a8b4-7bb2-4030-8e71-7ac55be3b39b");
    public static readonly Guid NotConfirmed = new("09ae6f25-4a41-4c3d-903d-64d1af3b6a90");

    public static readonly Guid BackgroundTaskExist = new("babbb71e-a368-492f-a1dc-74ebbec7db1a");
    public static readonly Guid BackgroundTaskNotExist = new("cf308b56-5cde-4127-9c65-b4f718ce820b");

    public static readonly Guid ResourceExist = new("e6df31eb-c361-4124-822a-abf1dfb060ad");
    public static readonly Guid ResourceNotExist = new("dea02423-9068-4dec-9d0d-5cd5d1db6a95");

    public static readonly Guid RoleExist = new("c60203c5-1efc-4043-9ab1-b57e04df0d53");
    public static readonly Guid RoleNotExist = new("035cce3a-bf42-457b-b806-4805238a1d44");

    public static readonly Guid UserActive = new("c8da8254-8c24-4ba9-b9d6-9742bae5d3d4");
    public static readonly Guid UserSuspend = new("a9aebfb1-a6ef-4dbb-8817-c1cf3d74d008");

    public static readonly Guid CountryNotExist = new("2dae1d83-af09-4a7d-86c4-137615221a85");

    public static readonly Guid PhoneNumberIsNotCorrect = new("e42b0d81-3701-432b-bc68-82702ce01df6");

    public static readonly Guid AddressExist = new("aa34f997-6bee-4b50-96b4-3d26d2566a6e");
    public static readonly Guid AddressNotExist = new("cbd068f9-c0c7-4522-a60f-f06b724c91dc");

    public static readonly Guid Exist = new("7e57132b-48b2-4853-a54a-5fba1134f344");
    public static readonly Guid NotExist = new("20bdf7a8-3942-4ec4-94bf-067bd5757182");

    public static readonly Guid UserAccessExist = new("43da25f4-a613-46d3-914e-c8625fd49c39");
    public static readonly Guid UserAccessNotExist = new("988b7262-62ca-4e5c-8124-8772ea533655");

    public static readonly Guid WrongLogin = new("10960006-011b-411b-8791-40b24d2b5536");


    public static string GuidConversion(this Guid key)
    {
        return key switch
        {
            var x when x.In(LowerThen) => "LowerThen",
            var x when x.In(GreaterThan) => "GreaterThan",
            var x when x.In(NotEmpty) => "NotEmpty",
            var x when x.In(NotNull) => "NotNull",
            var x when x.In(Equal) => "Equal",
            var x when x.In(NotEqual) => "NotEqual",
            var x when x.In(NullIntTimeout) => "NullIntTimeout",
            var x when x.In(EmailIsNotCorrect) => "EmailIsNotCorrect",
            var x when x.In(IpAddressIsNotCorrect) => "IpAddressIsNotCorrect",
            var x when x.In(UserSuspended) => "UserSuspended",
            var x when x.In(UserDeleted) => "UserDeleted",
            var x when x.In(NotConfirmed) => "NotConfirmed",
            var x when x.In(UserNotExist, BackgroundTaskNotExist, ResourceNotExist,
                RoleNotExist, CountryNotExist, AddressNotExist, NotExist, UserAccessNotExist) => "NotExist",
            var x when x.In(UserExist, BackgroundTaskExist, ResourceExist, RoleExist, AddressExist, Exist) => "Exist",
            var x when x.In(PasswordLowerChar, PasswordUpperChar, PasswordMiniMaxChars,
                PasswordNumber, PasswordSymbols, UserAccessExist) => "WrongPassword",
            var x when x.In(WrongLogin) => "WrongLogin",
            _ => "CustomError"
        };
    }

    private static bool In<T>(this T val, params T[] vals)
    {
        return vals.Contains(val);
    }
}