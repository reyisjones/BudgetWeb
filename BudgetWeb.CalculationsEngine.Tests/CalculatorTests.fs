module CalculatorTests

open System
open Xunit
open BudgetWeb.CalculationsEngine.MortgageCalculations
open BudgetWeb.CalculationsEngine.CarLoanCalculations
open BudgetWeb.CalculationsEngine.StudentLoanCalculations
open BudgetWeb.CalculationsEngine.DebtAnalysis
open BudgetWeb.CalculationsEngine.SavingsCalculations
open BudgetWeb.CalculationsEngine.RefinanceCalculations
open BudgetWeb.CalculationsEngine.BudgetCategoryAnalyzer


/// Mortgage Calculator Tests
module MortgageCalculatorTests =
    
    [<Fact>]
    let ``calculateMortgagePayment should calculate correct monthly payment for 30-year mortgage`` () =
        let principal = 300000m
        let annualRate = 4.5m
        let years = 30
        let payment = calculateMortgagePayment principal annualRate years Monthly
        
        // Expected: ~$1,520.06 monthly payment
        Assert.InRange(payment, 1519m, 1521m)
    
    [<Fact>]
    let ``calculateMortgagePayment should calculate correct payment for 15-year mortgage`` () =
        let principal = 300000m
        let annualRate = 3.5m
        let years = 15
        let payment = calculateMortgagePayment principal annualRate years Monthly
        
        // Expected: ~$2,144.65 monthly payment
        Assert.InRange(payment, 2143m, 2146m)
    
    [<Fact>]
    let ``calculateMortgagePayment should handle bi-weekly payment frequency`` () =
        let principal = 200000m
        let annualRate = 4.0m
        let years = 30
        let monthlyPayment = calculateMortgagePayment principal annualRate years Monthly
        let biWeeklyPayment = calculateMortgagePayment principal annualRate years BiWeekly
        
        // Bi-weekly payment should be roughly half of monthly
        Assert.InRange(biWeeklyPayment, monthlyPayment * 0.45m, monthlyPayment * 0.55m)
    
    [<Fact>]
    let ``generateMortgageSchedule should produce correct number of payments`` () =
        let principal = 100000m
        let annualRate = 5.0m
        let years = 30
        let schedule = generateMortgageSchedule principal annualRate years Monthly 0m
        
        Assert.Equal(360, schedule.Length) // 30 years * 12 months
    
    [<Fact>]
    let ``generateMortgageSchedule should show decreasing balance`` () =
        let principal = 100000m
        let annualRate = 5.0m
        let years = 30
        let schedule = generateMortgageSchedule principal annualRate years Monthly 0m
        
        let firstPayment = schedule |> List.head
        let lastPayment = schedule |> List.last
        
        Assert.Equal(principal, firstPayment.RemainingBalance + firstPayment.PrincipalPortion)
        Assert.InRange(lastPayment.RemainingBalance, 0m, 1m) // Should be paid off
    
    [<Fact>]
    let ``generateMortgageSchedule with extra payments should reduce total payments`` () =
        let principal = 200000m
        let annualRate = 4.0m
        let years = 30
        let regularSchedule = generateMortgageSchedule principal annualRate years Monthly 0m
        let acceleratedSchedule = generateMortgageSchedule principal annualRate years Monthly 200m
        
        Assert.True(acceleratedSchedule.Length < regularSchedule.Length)
    
    [<Fact>]
    let ``calculateMortgageSummary should calculate total interest correctly`` () =
        let principal = 250000m
        let annualRate = 4.0m
        let years = 30
        let summary = calculateMortgageSummary principal annualRate years Monthly 0m None
        
        Assert.True(summary.TotalInterest > 0m)
        // Allow small rounding difference
        Assert.InRange(summary.TotalPaid, summary.LoanAmount + summary.TotalInterest - 0.01m, summary.LoanAmount + summary.TotalInterest + 0.01m)
    
    [<Fact>]
    let ``calculatePayoffAcceleration should show interest savings with extra payments`` () =
        let principal = 300000m
        let annualRate = 4.5m
        let years = 30
        let extraPayment = 500m
        
        let (paymentsSaved, interestSaved, _) = calculatePayoffAcceleration principal annualRate years Monthly extraPayment
        
        Assert.True(paymentsSaved > 0)
        Assert.True(interestSaved > 0m)


/// Car Loan Calculator Tests
module CarLoanCalculatorTests =
    
    [<Fact>]
    let ``calculateCarLoan should include sales tax and fees`` () =
        let vehiclePrice = 30000m
        let downPayment = 5000m
        let tradeIn = 3000m
        let salesTaxRate = 8.0m
        let fees = 500m
        let annualRate = 5.0m
        let termMonths = 60
        
        let loan = calculateCarLoan vehiclePrice downPayment tradeIn salesTaxRate fees annualRate termMonths
        
        Assert.Equal(vehiclePrice, loan.VehiclePrice)
        Assert.Equal(vehiclePrice * 0.08m, loan.SalesTax)
        Assert.Equal(fees, loan.Fees)
        Assert.True(loan.LoanAmount > 0m)
    
    [<Fact>]
    let ``calculateCarLoan should calculate correct loan amount`` () =
        let vehiclePrice = 25000m
        let downPayment = 5000m
        let tradeIn = 2000m
        let salesTaxRate = 7.0m
        let fees = 300m
        let annualRate = 4.5m
        let termMonths = 48
        
        let loan = calculateCarLoan vehiclePrice downPayment tradeIn salesTaxRate fees annualRate termMonths
        
        let expectedLoanAmount = vehiclePrice + (vehiclePrice * 0.07m) + fees - downPayment - tradeIn
        Assert.Equal(expectedLoanAmount, loan.LoanAmount)
    
    [<Fact>]
    let ``calculateCarLoan should calculate monthly payment`` () =
        let vehiclePrice = 20000m
        let downPayment = 3000m
        let tradeIn = 0m
        let salesTaxRate = 6.0m
        let fees = 200m
        let annualRate = 5.5m
        let termMonths = 60
        
        let loan = calculateCarLoan vehiclePrice downPayment tradeIn salesTaxRate fees annualRate termMonths
        
        Assert.True(loan.MonthlyPayment > 0m)
        Assert.True(loan.MonthlyPayment < loan.LoanAmount) // Monthly payment should be less than loan amount
    
    [<Fact>]
    let ``compareLeaseToBuy should show lease vs buy costs`` () =
        let vehiclePrice = 35000m
        let leaseMonthly = 400m
        let leaseTerm = 36
        let buyMonthly = 650m
        let buyTerm = 60
        let residualValue = 15000m
        
        let (leaseCost, buyCost, equity) = compareLeaseToBuy vehiclePrice leaseMonthly leaseTerm buyMonthly buyTerm residualValue
        
        Assert.True(leaseCost > 0m)
        Assert.True(buyCost > 0m)
        Assert.True(equity > 0m)


/// Student Loan Calculator Tests
module StudentLoanCalculatorTests =
    
    [<Fact>]
    let ``calculateStudentLoanPayment should calculate standard 10-year payment`` () =
        let principal = 30000m
        let annualRate = 5.0m
        let termYears = 10
        
        let payment = calculateStudentLoanPayment principal annualRate termYears
        
        Assert.True(payment > 0m)
        Assert.InRange(payment, 310m, 325m) // ~$318 expected
    
    [<Fact>]
    let ``calculateIncomeBasedPayment should calculate based on income and family size`` () =
        let annualIncome = 50000m
        let familySize = 1
        let discretionaryPercent = 10m
        
        let payment = calculateIncomeBasedPayment annualIncome familySize discretionaryPercent
        
        Assert.True(payment >= 0m)
    
    [<Fact>]
    let ``calculateIncomeBasedPayment should return zero for low income`` () =
        let annualIncome = 20000m
        let familySize = 2
        let discretionaryPercent = 10m
        
        let payment = calculateIncomeBasedPayment annualIncome familySize discretionaryPercent
        
        Assert.Equal(0m, payment)
    
    [<Fact>]
    let ``calculateStudentLoanWithDeferment should capitalize interest`` () =
        let principal = 25000m
        let annualRate = 6.0m
        let gracePeriod = 6
        let deferment = 12
        let termYears = 10
        
        let summary = calculateStudentLoanWithDeferment principal annualRate gracePeriod deferment termYears
        
        Assert.True(summary.InterestCapitalization > 0m)
        Assert.True(summary.LoanBalance > principal)
    
    [<Fact>]
    let ``calculateLoanForgiveness should calculate remaining balance after forgiveness period`` () =
        let principal = 50000m
        let annualRate = 5.5m
        let monthlyPayment = 300m
        let forgivenessMonths = 120 // 10 years for PSLF
        
        let (remainingBalance, totalPaid, totalInterest) = calculateLoanForgiveness principal annualRate monthlyPayment forgivenessMonths
        
        Assert.True(totalPaid > 0m)
        Assert.True(totalInterest > 0m)


/// Debt Analysis Tests
module DebtAnalysisTests =
    
    [<Fact>]
    let ``calculateDebtToIncomeRatio should return correct percentage`` () =
        let monthlyDebt = 2000m
        let monthlyIncome = 5000m
        
        let dti = calculateDebtToIncomeRatio monthlyDebt monthlyIncome
        
        Assert.Equal(Some 40m, dti)
    
    [<Fact>]
    let ``calculateDebtToIncomeRatio should return None for zero income`` () =
        let monthlyDebt = 1000m
        let monthlyIncome = 0m
        
        let dti = calculateDebtToIncomeRatio monthlyDebt monthlyIncome
        
        Assert.Equal(None, dti)
    
    [<Fact>]
    let ``calculateWeightedAverageRate should calculate correct average`` () =
        let debts = [
            { Name = "Credit Card 1"; Balance = 5000m; InterestRate = 18.0m; MinimumPayment = 150m }
            { Name = "Credit Card 2"; Balance = 3000m; InterestRate = 15.0m; MinimumPayment = 90m }
            { Name = "Car Loan"; Balance = 12000m; InterestRate = 5.0m; MinimumPayment = 250m }
        ]
        
        let avgRate = calculateWeightedAverageRate debts
        
        Assert.True(avgRate > 0m)
        Assert.True(avgRate < 18.0m)
    
    [<Fact>]
    let ``debtAvalanche should prioritize highest interest debt`` () =
        let debts = [
            { Name = "Low Interest"; Balance = 10000m; InterestRate = 3.0m; MinimumPayment = 100m }
            { Name = "High Interest"; Balance = 5000m; InterestRate = 20.0m; MinimumPayment = 150m }
        ]
        let extraPayment = 200m
        
        let results = debtAvalanche debts extraPayment
        
        Assert.Equal(2, results.Length)
        let (firstDebtName, _, _) = results |> List.head
        Assert.Equal("High Interest", firstDebtName)


/// Savings Calculator Tests
module SavingsCalculatorTests =
    
    [<Fact>]
    let ``calculateSavingsGoal should determine months to reach goal`` () =
        let target = 10000m
        let current = 1000m
        let monthly = 500m
        let rate = 5.0m
        
        let goal = calculateSavingsGoal target current monthly rate
        
        Assert.True(goal.IsSome)
        match goal with
        | Some g ->
            Assert.True(g.MonthsToGoal > 0)
            Assert.True(g.MonthsToGoal < 24) // Should reach in under 2 years
        | None -> Assert.True(false, "Should have calculated a goal")
    
    [<Fact>]
    let ``calculateInvestmentProjection should calculate future value with contributions`` () =
        let initial = 10000m
        let monthly = 500m
        let rate = 7.0m
        let years = 10
        
        let projection = calculateInvestmentProjection initial monthly rate years
        
        Assert.True(projection.FutureValue > initial)
        Assert.True(projection.TotalGains > 0m)
        Assert.Equal(monthly * decimal (years * 12), projection.TotalContributions)
    
    [<Fact>]
    let ``calculateCompoundInterestAdvanced should handle different compounding frequencies`` () =
        let principal = 10000m
        let rate = 5.0m
        let years = 10
        
        let annual = calculateCompoundInterestAdvanced principal rate years 1
        let monthly = calculateCompoundInterestAdvanced principal rate years 12
        let daily = calculateCompoundInterestAdvanced principal rate years 365
        
        Assert.True(monthly > annual)
        Assert.True(daily > monthly)
    
    [<Fact>]
    let ``calculateRequiredMonthlySavings should calculate needed monthly contribution`` () =
        let target = 50000m
        let current = 10000m
        let rate = 6.0m
        let years = 10
        
        let required = calculateRequiredMonthlySavings target current rate years
        
        Assert.True(required.IsSome)
        match required with
        | Some amount ->
            Assert.True(amount > 0m)
            Assert.True(amount < 500m) // Sanity check
        | None -> Assert.True(false, "Should calculate required savings")


/// Refinance Calculator Tests
module RefinanceCalculatorTests =
    
    [<Fact>]
    let ``compareRefinance should show savings when refinancing to lower rate`` () =
        let currentBalance = 200000m
        let currentRate = 6.0m
        let currentRemainingMonths = 300 // 25 years left
        let newRate = 4.0m
        let newTermYears = 25
        let closingCosts = 5000m
        
        let comparison = compareRefinance currentBalance currentRate currentRemainingMonths newRate newTermYears closingCosts
        
        Assert.True(comparison.MonthlyPaymentSavings > 0m)
        Assert.True(comparison.TotalInterestSavings > 0m)
    
    [<Fact>]
    let ``compareRefinance should calculate break-even point`` () =
        let currentBalance = 150000m
        let currentRate = 5.5m
        let currentRemainingMonths = 240
        let newRate = 4.0m
        let newTermYears = 20
        let closingCosts = 3000m
        
        let comparison = compareRefinance currentBalance currentRate currentRemainingMonths newRate newTermYears closingCosts
        
        Assert.True(comparison.BreakEvenMonths > 0)
        Assert.True(comparison.BreakEvenMonths < 1000) // Sanity check
    
    [<Fact>]
    let ``compareRefinance should recommend refinance when worthwhile`` () =
        let currentBalance = 300000m
        let currentRate = 7.0m
        let currentRemainingMonths = 360
        let newRate = 4.5m
        let newTermYears = 30
        let closingCosts = 4000m
        
        let comparison = compareRefinance currentBalance currentRate currentRemainingMonths newRate newTermYears closingCosts
        
        Assert.True(comparison.IsWorthwhile)
        Assert.True(comparison.NetSavings > 0m)
    
    [<Fact>]
    let ``calculateEffectiveRate should include fees in rate calculation`` () =
        let loanAmount = 200000m
        let nominalRate = 4.0m
        let fees = 4000m
        let termYears = 30
        
        let effectiveRate = calculateEffectiveRate loanAmount nominalRate fees termYears
        
        Assert.True(effectiveRate > nominalRate)


/// Budget Category Analyzer Tests
module BudgetCategoryAnalyzerTests =
    
    [<Fact>]
    let ``analyzeCategorySpending should identify overspent categories`` () =
        let categories = [
            ("Housing", 2000m, 2100m)
            ("Food", 600m, 700m)
            ("Transportation", 400m, 350m)
        ]
        
        let analysis = analyzeCategorySpending categories
        
        Assert.True(analysis.TotalVariance > 0m)
        Assert.True(analysis.OverspentCategories.Length > 0)
    
    [<Fact>]
    let ``analyzeCategorySpending should identify underspent categories`` () =
        let categories = [
            ("Housing", 2000m, 1900m)
            ("Food", 600m, 500m)
            ("Transportation", 400m, 400m)
        ]
        
        let analysis = analyzeCategorySpending categories
        
        Assert.True(analysis.TotalVariance < 0m)
        Assert.True(analysis.UnderspentCategories.Length > 0)
    
    [<Fact>]
    let ``analyzeCategorySpending should provide recommendations`` () =
        let categories = [
            ("Housing", 2000m, 2200m)
            ("Food", 600m, 700m)
            ("Transportation", 400m, 350m)
        ]
        
        let analysis = analyzeCategorySpending categories
        
        Assert.True(analysis.Recommendations.Length > 0)
    
    [<Fact>]
    let ``recommendBudgetAdjustments should proportionally adjust categories`` () =
        let categories = [
            ("Housing", 2000m, 2000m)
            ("Food", 500m, 500m)
            ("Transportation", 300m, 300m)
        ]
        let targetTotal = 3080m // 10% increase
        
        let adjustments = recommendBudgetAdjustments categories targetTotal
        
        Assert.Equal(3, adjustments.Length)
        let totalAdjusted = adjustments |> List.sumBy (fun (_, adjusted, _) -> adjusted)
        Assert.InRange(totalAdjusted, 3079m, 3081m)
