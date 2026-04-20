// =============================================================================
// Hands-On 003 — Clean Up OrderService
// =============================================================================
// TASK: This file contains numerous Clean Code violations.
//
// Step 1 — Naming: identify every identifier below that violates C# conventions
//          and list them in a comment block at the top of your refactored file.
//          Hint: look for Hungarian notation, cryptic abbreviations, and
//          misleading names.
//
// Step 2 — Extract repository: pull the inline SQL out of ProcessOrd() into a
//          dedicated repository class/method with a meaningful name.
//
// Step 3 — Magic numbers: replace the three magic decimal literals with named
//          constants inside a new PricingPolicy class.
//
// Step 4 — Split the method: ProcessOrd(o, em, s, log) does four things.
//          Give each concern its own focused method.
//
// Step 5 — Result pattern: replace the `out string err` parameter with a
//          Result<Order> return type (use the record from the presentation or
//          a minimal inline version).
//
// Stretch goal: convert Address from a mutable class to a record and update
//               all usages.
// =============================================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;

// ----------------------------- Domain model ----------------------------------

public class ord   // ❌ naming: cryptic abbreviation
{
    public int    ordId  { get; set; }         // ❌ Hungarian-ish, camelCase on property
    public string cEmail { get; set; } = "";   // ❌ abbreviation
    public List<itm> itms { get; set; } = [];  // ❌ cryptic
    public Address   addr  { get; set; } = new Address();
}

public class itm   // ❌ naming: cryptic abbreviation
{
    public string nm  { get; set; } = "";  // ❌ name
    public int    qty { get; set; }        // ❌ acceptable in tight loops — not as a property name
    public decimal p  { get; set; }        // ❌ 'p' for price
}

public class Address   // mutable class — stretch goal target
{
    public string Str  { get; set; } = "";   // ❌ abbreviation for Street
    public string Cty  { get; set; } = "";   // ❌ abbreviation for City
    public string Zip  { get; set; } = "";
}

// ----------------------------- Service ---------------------------------------

public class OrdSvc   // ❌ naming: abbreviation
{
    private string CS = "Server=.;Database=Shop;Integrated Security=true;"; // ❌ 'CS' for connection string; field should be _camelCase

    // ❌ flag arguments, out parameter, abbreviated names, mixed abstraction levels
    public void ProcessOrd(ord o, bool em, bool s, bool log, out string err)
    {
        err = "";

        // --- Validate ---
        if (o == null)          { err = "order is null";   return; }
        if (o.itms.Count == 0)  { err = "no items";        return; }
        if (string.IsNullOrEmpty(o.cEmail)) { err = "no email"; return; }

        var x = o.itms.Where(i => i.qty > 0).ToList();   // ❌ 'x' is meaningless

        // --- Calculate ---
        decimal amt = x.Sum(i => i.p * i.qty);   // ❌ 'amt' abbreviation

        decimal fee;                               // ❌ magic numbers below
        if (s)
            fee = amt * 0.085m + 4.99m;            // ❌ what is 0.085? what is 4.99?
        else
            fee = amt * 0.085m;                    // ❌ same magic rate, duplicated

        decimal tot = amt + fee;                   // ❌ 'tot' abbreviation

        // --- Persist (inline SQL) --- ❌ SQL in service method, not parameterised safely
        using (var conn = new SqlConnection(CS))
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "INSERT INTO Orders (Id, CustomerEmail, Total, Fee, ShippingAddress) " +
                "VALUES ('" + o.ordId + "','" + o.cEmail + "'," + tot + "," + fee + ",'" + o.addr.Str + "')";
            // ❌ string concatenation — SQL injection vulnerability
            cmd.ExecuteNonQuery();

            foreach (var i in x)
            {
                var c2 = conn.CreateCommand();     // ❌ 'c2' is meaningless
                c2.CommandText =
                    "INSERT INTO OrderItems (OrderId, Name, Qty, Price) VALUES (" +
                    o.ordId + ",'" + i.nm + "'," + i.qty + "," + i.p + ")";
                c2.ExecuteNonQuery();
            }
        }

        // --- Notify (email) ---
        if (em)   // ❌ 'em' — flag argument masking what it controls
        {
            var smtp = new SmtpClient("mail.example.com");   // ❌ hard-coded server
            smtp.Send(
                "noreply@shop.com",
                o.cEmail,
                "Your order #" + o.ordId,
                "Total: " + tot);
        }

        // --- SMS ---
        if (s)    // ❌ 's' — flag argument, same name used for shipping-surcharge check above
        {
            // Pretend we call an SMS API
            File.AppendAllText("sms_queue.txt", o.cEmail + "|Order #" + o.ordId + " placed\n");
        }

        // --- Audit log ---
        if (log)
        {
            File.AppendAllText(
                "audit.log",
                DateTime.Now + " | order " + o.ordId + " | " + tot + "\n");  // ❌ DateTime.Now (not UTC)
        }
    }
}
