using S_B_MediaApi.Models;


namespace S_B_MediaApi.Data;


public static class DemoData
{
public static List<Consumer> Consumers { get; } = new()
{
new Consumer{ Id = 1, ContractId = 1, Name = "Default Teilnehmer", FilialId = 2010 },
new Consumer{ Id = 2, ContractId = 1, Name = "Default Teilnehmer", FilialId = 2010 }
};


public static List<ConsumerDetail> ConsumerDetails { get; } = new()
{
new ConsumerDetail
{
Consumer = new Consumer{ Id = 1, ContractId = 1, Name = "Default Teilnehmer", FilialId = 2010 },
Person = new Person{ FirstName = "TN", Surname = "Default Teilnehmer" },
Identification = new Identification{ PtcptType = 2, CardNo = "656456546546" },
Status = 6, Delete = 0
}
};


public static List<Invoice> Invoices { get; } = new()
{
new Invoice{ Id = 65, InvcDate = DateTime.UtcNow.AddDays(-7), InvcAmount = 200, CompanyRef = 1, CompanyUrl = "contracts/1" },
new Invoice{ Id = 66, InvcDate = DateTime.UtcNow.AddDays(-3), InvcAmount = 1200, CompanyRef = 1, CompanyUrl = "contracts/1" }
};


public static List<SalesTransaction> Sales { get; } = new();
public static List<ParkTransaction> Park { get; } = new();
}