namespace BudgetWeb.CalculationsEngine

open System

/// Budget variance and comparison calculations
module BudgetVariance =
    
    /// Calculate absolute variance between actual and budgeted amounts
    let calculateVariance (actual: decimal) (budgeted: decimal) : decimal =
        actual - budgeted
    
    /// Calculate percentage variance
    let calculateVariancePercentage (actual: decimal) (budgeted: decimal) : decimal option =
        if budgeted = 0m then None
        else Some ((actual - budgeted) / budgeted * 100m)
    
    /// Determine if budget is over, under, or on target
    type VarianceStatus = Over | Under | OnTarget
    
    let getVarianceStatus (actual: decimal) (budgeted: decimal) (tolerance: decimal) : VarianceStatus =
        let variance = calculateVariance actual budgeted
        let absVariance = abs variance
        if absVariance <= tolerance then OnTarget
        elif variance > 0m then Over
        else Under
    
    /// Calculate budget utilization rate
    let calculateUtilizationRate (spent: decimal) (budgeted: decimal) : decimal option =
        if budgeted = 0m then None
        else Some (spent / budgeted * 100m)
    
    /// Calculate remaining budget
    let calculateRemainingBudget (budgeted: decimal) (spent: decimal) : decimal =
        budgeted - spent
    
    /// Calculate burn rate (spending rate per period)
    let calculateBurnRate (totalSpent: decimal) (periodsElapsed: int) : decimal =
        if periodsElapsed = 0 then 0m
        else totalSpent / decimal periodsElapsed


/// Financial forecasting and projection calculations
module Forecasting =
    
    /// Simple linear forecast based on historical trend
    let linearForecast (historicalValues: decimal list) (periodsAhead: int) : decimal list =
        let n = historicalValues.Length |> float
        if n < 2.0 then []
        else
            // Calculate linear regression: y = mx + b
            let indices = [0.0 .. n - 1.0]
            let avgX = List.average indices
            let avgY = List.average (List.map float historicalValues)
            
            let numerator = 
                List.zip indices (List.map float historicalValues)
                |> List.sumBy (fun (x, y) -> (x - avgX) * (y - avgY))
            
            let denominator = 
                indices
                |> List.sumBy (fun x -> (x - avgX) ** 2.0)
            
            let slope = if denominator = 0.0 then 0.0 else numerator / denominator
            let intercept = avgY - slope * avgX
            
            // Generate forecasts
            [1..periodsAhead]
            |> List.map (fun p -> 
                let futureIndex = n + float p - 1.0
                decimal (slope * futureIndex + intercept))
    
    /// Moving average forecast
    let movingAverageForecast (historicalValues: decimal list) (windowSize: int) (periodsAhead: int) : decimal =
        if historicalValues.Length < windowSize then 0m
        else
            historicalValues
            |> List.rev
            |> List.take windowSize
            |> List.average
    
    /// Exponential smoothing forecast
    let exponentialSmoothing (historicalValues: decimal list) (alpha: float) (periodsAhead: int) : decimal list =
        if historicalValues.IsEmpty then []
        else
            let rec smooth acc remaining =
                match remaining with
                | [] -> acc
                | x :: xs ->
                    let lastForecast = List.head acc
                    let newForecast = decimal alpha * x + decimal (1.0 - alpha) * lastForecast
                    smooth (newForecast :: acc) xs
            
            let initial = List.head historicalValues
            let smoothed = smooth [initial] (List.tail historicalValues) |> List.rev
            
            // Forecast ahead using last smoothed value
            List.replicate periodsAhead (List.last smoothed)
    
    /// Calculate trend direction
    type TrendDirection = Increasing | Decreasing | Stable
    
    let identifyTrend (values: decimal list) : TrendDirection =
        if values.Length < 2 then Stable
        else
            let differences = 
                values
                |> List.pairwise
                |> List.map (fun (a, b) -> b - a)
            
            let avgDiff = List.average differences
            if avgDiff > 0.1m then Increasing
            elif avgDiff < -0.1m then Decreasing
            else Stable


/// Cash flow analysis and projections
module CashFlow =
    
    /// Calculate net cash flow
    let calculateNetCashFlow (inflows: decimal list) (outflows: decimal list) : decimal =
        List.sum inflows - List.sum outflows
    
    /// Calculate cumulative cash flow over time
    let calculateCumulativeCashFlow (netFlows: decimal list) : decimal list =
        netFlows
        |> List.scan (+) 0m
        |> List.tail
    
    /// Calculate cash flow coverage ratio
    let cashFlowCoverageRatio (operatingCashFlow: decimal) (totalDebtService: decimal) : decimal option =
        if totalDebtService = 0m then None
        else Some (operatingCashFlow / totalDebtService)
    
    /// Calculate operating cash flow ratio
    let operatingCashFlowRatio (operatingCashFlow: decimal) (currentLiabilities: decimal) : decimal option =
        if currentLiabilities = 0m then None
        else Some (operatingCashFlow / currentLiabilities)
    
    /// Calculate free cash flow
    let calculateFreeCashFlow (operatingCashFlow: decimal) (capitalExpenditures: decimal) : decimal =
        operatingCashFlow - capitalExpenditures
    
    /// Project future cash position
    let projectCashPosition (startingCash: decimal) (projectedInflows: decimal list) (projectedOutflows: decimal list) : decimal list =
        let netFlows = List.map2 (-) projectedInflows projectedOutflows
        netFlows
        |> List.scan (+) startingCash
        |> List.tail
    
    /// Calculate days of cash on hand
    let daysOfCashOnHand (cashAndEquivalents: decimal) (dailyCashExpenses: decimal) : decimal option =
        if dailyCashExpenses = 0m then None
        else Some (cashAndEquivalents / dailyCashExpenses)


/// Return on Investment (ROI) calculations
module ROI =
    
    /// Helper function for decimal power calculation
    let private decimalPower (baseValue: decimal) (exponent: int) : decimal =
        let rec power acc exp =
            if exp <= 0 then acc
            else power (acc * baseValue) (exp - 1)
        if exponent >= 0 then power 1m exponent
        else 1m / power 1m (-exponent)
    
    /// Calculate simple ROI
    let calculateROI (gain: decimal) (cost: decimal) : decimal option =
        if cost = 0m then None
        else Some ((gain - cost) / cost * 100m)
    
    /// Calculate Return on Assets (ROA)
    let calculateROA (netIncome: decimal) (totalAssets: decimal) : decimal option =
        if totalAssets = 0m then None
        else Some (netIncome / totalAssets * 100m)
    
    /// Calculate Return on Equity (ROE)
    let calculateROE (netIncome: decimal) (shareholderEquity: decimal) : decimal option =
        if shareholderEquity = 0m then None
        else Some (netIncome / shareholderEquity * 100m)
    
    /// Calculate Internal Rate of Return (IRR) - simplified Newton-Raphson method
    let calculateIRR (cashFlows: decimal list) (maxIterations: int) (tolerance: float) : decimal option =
        let rec newton rate iteration =
            if iteration >= maxIterations then None
            else
                let r = float rate
                let npv = 
                    cashFlows
                    |> List.mapi (fun i cf -> float cf / (1.0 + r) ** float i)
                    |> List.sum
                
                let dnpv = 
                    cashFlows
                    |> List.mapi (fun i cf -> -float i * float cf / (1.0 + r) ** (float i + 1.0))
                    |> List.sum
                
                if abs npv < tolerance then Some (decimal (r * 100.0))
                elif dnpv = 0.0 then None
                else
                    let newRate = decimal (r - npv / dnpv)
                    newton newRate (iteration + 1)
        
        newton 0.1m 0
    
    /// Calculate Net Present Value (NPV)
    let calculateNPV (discountRate: decimal) (cashFlows: decimal list) : decimal =
        cashFlows
        |> List.mapi (fun i cf -> cf / decimalPower (1m + discountRate) i)
        |> List.sum
    
    /// Calculate Payback Period
    let calculatePaybackPeriod (initialInvestment: decimal) (cashFlows: decimal list) : int option =
        let cumulative = CashFlow.calculateCumulativeCashFlow cashFlows
        cumulative
        |> List.tryFindIndex (fun cf -> cf >= initialInvestment)
        |> Option.map ((+) 1)
    
    /// Calculate profitability index
    let calculateProfitabilityIndex (presentValueOfFutureCashFlows: decimal) (initialInvestment: decimal) : decimal option =
        if initialInvestment = 0m then None
        else Some (presentValueOfFutureCashFlows / initialInvestment)


/// Compound interest and amortization calculations
module InterestCalculations =
    
    /// Helper function for decimal power calculation
    let private decimalPower (baseValue: decimal) (exponent: float) : decimal =
        decimal (float baseValue ** exponent)
    
    /// Calculate compound interest
    let calculateCompoundInterest (principal: decimal) (annualRate: decimal) (timesCompounded: int) (years: decimal) : decimal =
        let rate = 1m + (annualRate / decimal timesCompounded)
        let periods = float timesCompounded * float years
        principal * decimalPower rate periods
    
    /// Calculate simple interest
    let calculateSimpleInterest (principal: decimal) (rate: decimal) (time: decimal) : decimal =
        principal * rate * time
    
    /// Calculate future value of an investment
    let calculateFutureValue (presentValue: decimal) (interestRate: decimal) (periods: int) : decimal =
        presentValue * decimalPower (1m + interestRate) (float periods)
    
    /// Calculate present value
    let calculatePresentValue (futureValue: decimal) (interestRate: decimal) (periods: int) : decimal =
        futureValue / decimalPower (1m + interestRate) (float periods)
    
    /// Calculate loan payment (amortization)
    let calculateLoanPayment (principal: decimal) (annualRate: decimal) (numberOfPayments: int) : decimal =
        if annualRate = 0m then principal / decimal numberOfPayments
        else
            let monthlyRate = annualRate / 12m
            let factor = decimalPower (1m + monthlyRate) (float numberOfPayments)
            principal * (monthlyRate * factor) / (factor - 1m)
    
    /// Calculate remaining loan balance
    let calculateRemainingBalance (principal: decimal) (annualRate: decimal) (totalPayments: int) (paymentsMade: int) : decimal =
        let payment = calculateLoanPayment principal annualRate totalPayments
        let monthlyRate = annualRate / 12m
        let remainingPayments = totalPayments - paymentsMade
        if monthlyRate = 0m then principal - (payment * decimal paymentsMade)
        else
            let factor = decimalPower (1m + monthlyRate) (float remainingPayments)
            payment * (factor - 1m) / (monthlyRate * factor)
    
    /// Calculate total interest paid over loan life
    let calculateTotalInterest (principal: decimal) (annualRate: decimal) (numberOfPayments: int) : decimal =
        let payment = calculateLoanPayment principal annualRate numberOfPayments
        payment * decimal numberOfPayments - principal
    
    /// Generate amortization schedule
    type AmortizationEntry = {
        Period: int
        Payment: decimal
        Principal: decimal
        Interest: decimal
        Balance: decimal
    }
    
    let generateAmortizationSchedule (principal: decimal) (annualRate: decimal) (numberOfPayments: int) : AmortizationEntry list =
        let payment = calculateLoanPayment principal annualRate numberOfPayments
        let monthlyRate = annualRate / 12m
        
        let rec buildSchedule period balance acc =
            if period > numberOfPayments then List.rev acc
            else
                let interest = balance * monthlyRate
                let principalPayment = payment - interest
                let newBalance = balance - principalPayment
                let entry = {
                    Period = period
                    Payment = payment
                    Principal = principalPayment
                    Interest = interest
                    Balance = max 0m newBalance
                }
                buildSchedule (period + 1) newBalance (entry :: acc)
        
        buildSchedule 1 principal []


/// Project cost estimation calculations
module ProjectEstimation =
    
    /// Three-point estimation (PERT)
    let threePointEstimate (optimistic: decimal) (mostLikely: decimal) (pessimistic: decimal) : decimal =
        (optimistic + 4m * mostLikely + pessimistic) / 6m
    
    /// Calculate standard deviation for three-point estimate
    let threePointStandardDeviation (optimistic: decimal) (pessimistic: decimal) : decimal =
        (pessimistic - optimistic) / 6m
    
    /// Calculate confidence interval
    let calculateConfidenceInterval (estimate: decimal) (standardDeviation: decimal) (confidenceLevel: float) : decimal * decimal =
        // Z-scores for common confidence levels
        let zScore = 
            match confidenceLevel with
            | 0.68 -> 1.0
            | 0.95 -> 1.96
            | 0.99 -> 2.58
            | _ -> 1.96
        
        let margin = decimal zScore * standardDeviation
        (estimate - margin, estimate + margin)
    
    /// Calculate Earned Value Management (EVM) metrics
    type EVMMetrics = {
        PlannedValue: decimal
        EarnedValue: decimal
        ActualCost: decimal
        ScheduleVariance: decimal
        CostVariance: decimal
        SchedulePerformanceIndex: decimal option
        CostPerformanceIndex: decimal option
        EstimateAtCompletion: decimal option
        EstimateToComplete: decimal option
    }
    
    let calculateEVMMetrics (plannedValue: decimal) (earnedValue: decimal) (actualCost: decimal) (budgetAtCompletion: decimal) : EVMMetrics =
        let scheduleVariance = earnedValue - plannedValue
        let costVariance = earnedValue - actualCost
        let spi = if plannedValue = 0m then None else Some (earnedValue / plannedValue)
        let cpi = if actualCost = 0m then None else Some (earnedValue / actualCost)
        let eac = 
            match cpi with
            | Some c when c <> 0m -> Some (budgetAtCompletion / c)
            | _ -> None
        let etc = 
            match eac with
            | Some e -> Some (e - actualCost)
            | None -> None
        
        {
            PlannedValue = plannedValue
            EarnedValue = earnedValue
            ActualCost = actualCost
            ScheduleVariance = scheduleVariance
            CostVariance = costVariance
            SchedulePerformanceIndex = spi
            CostPerformanceIndex = cpi
            EstimateAtCompletion = eac
            EstimateToComplete = etc
        }
    
    /// Calculate contingency reserve
    let calculateContingencyReserve (baseEstimate: decimal) (riskPercentage: decimal) : decimal =
        baseEstimate * (riskPercentage / 100m)
    
    /// Bottom-up cost estimation
    let bottomUpEstimate (taskCosts: decimal list) (contingencyPercent: decimal) : decimal =
        let baseTotal = List.sum taskCosts
        let contingency = calculateContingencyReserve baseTotal contingencyPercent
        baseTotal + contingency


/// Budget allocation and optimization
module BudgetOptimization =
    
    /// Calculate proportional allocation
    let proportionalAllocation (totalBudget: decimal) (weights: decimal list) : decimal list =
        let totalWeight = List.sum weights
        if totalWeight = 0m then List.replicate weights.Length 0m
        else
            weights |> List.map (fun w -> totalBudget * (w / totalWeight))
    
    /// Calculate priority-based allocation
    let priorityBasedAllocation (totalBudget: decimal) (priorities: (string * decimal * int) list) : (string * decimal) list =
        // priorities is list of (name, minimumRequired, priority)
        // Lower priority number = higher priority
        let sorted = priorities |> List.sortBy (fun (_, _, p) -> p)
        
        let rec allocate remaining items acc =
            match items with
            | [] -> List.rev acc
            | (name, minReq, _) :: rest ->
                let allocated = min minReq remaining
                let newRemaining = remaining - allocated
                allocate newRemaining rest ((name, allocated) :: acc)
        
        allocate totalBudget sorted []
    
    /// Calculate break-even point
    let calculateBreakEvenPoint (fixedCosts: decimal) (pricePerUnit: decimal) (variableCostPerUnit: decimal) : decimal option =
        let contributionMargin = pricePerUnit - variableCostPerUnit
        if contributionMargin <= 0m then None
        else Some (fixedCosts / contributionMargin)
    
    /// Calculate contribution margin ratio
    let contributionMarginRatio (revenue: decimal) (variableCosts: decimal) : decimal option =
        if revenue = 0m then None
        else Some ((revenue - variableCosts) / revenue * 100m)


/// Mortgage and loan calculations
module MortgageCalculations =
    
    type PaymentFrequency = Monthly | BiWeekly | Weekly
    
    type MortgagePayment = {
        PaymentNumber: int
        PaymentAmount: decimal
        PrincipalPortion: decimal
        InterestPortion: decimal
        RemainingBalance: decimal
        CumulativeInterest: decimal
        CumulativePrincipal: decimal
    }
    
    type MortgageSummary = {
        LoanAmount: decimal
        InterestRate: decimal
        TermYears: int
        PaymentFrequency: PaymentFrequency
        RegularPayment: decimal
        TotalPayments: int
        TotalInterest: decimal
        TotalPaid: decimal
        PayoffDate: DateTime option
    }
    
    /// Calculate mortgage payment amount
    let calculateMortgagePayment (principal: decimal) (annualRate: decimal) (years: int) (frequency: PaymentFrequency) : decimal =
        if principal <= 0m || annualRate < 0m || years <= 0 then 0m
        else
            let (periodsPerYear, paymentsTotal) = 
                match frequency with
                | Monthly -> (12, years * 12)
                | BiWeekly -> (26, years * 26)
                | Weekly -> (52, years * 52)
            
            let periodicRate = annualRate / decimal periodsPerYear / 100m
            
            if periodicRate = 0m then
                principal / decimal paymentsTotal
            else
                let rate = float periodicRate
                let n = float paymentsTotal
                let p = float principal
                let payment = p * (rate * (1.0 + rate) ** n) / ((1.0 + rate) ** n - 1.0)
                decimal payment
    
    /// Generate full mortgage amortization schedule
    let generateMortgageSchedule (principal: decimal) (annualRate: decimal) (years: int) (frequency: PaymentFrequency) (extraPayment: decimal) : MortgagePayment list =
        let regularPayment = calculateMortgagePayment principal annualRate years frequency
        let totalPayment = regularPayment + extraPayment
        
        let (periodsPerYear, _) = 
            match frequency with
            | Monthly -> (12, years * 12)
            | BiWeekly -> (26, years * 26)
            | Weekly -> (52, years * 52)
        
        let periodicRate = annualRate / decimal periodsPerYear / 100m
        
        let rec buildSchedule paymentNum balance cumulativeInt cumulativePrin acc =
            if balance <= 0m then List.rev acc
            else
                let interestPortion = balance * periodicRate
                let principalPortion = min (totalPayment - interestPortion) balance
                let actualPayment = principalPortion + interestPortion
                let newBalance = balance - principalPortion
                let newCumulativeInt = cumulativeInt + interestPortion
                let newCumulativePrin = cumulativePrin + principalPortion
                
                let payment = {
                    PaymentNumber = paymentNum
                    PaymentAmount = actualPayment
                    PrincipalPortion = principalPortion
                    InterestPortion = interestPortion
                    RemainingBalance = newBalance
                    CumulativeInterest = newCumulativeInt
                    CumulativePrincipal = newCumulativePrin
                }
                
                buildSchedule (paymentNum + 1) newBalance newCumulativeInt newCumulativePrin (payment :: acc)
        
        buildSchedule 1 principal 0m 0m []
    
    /// Calculate mortgage summary with extra payments
    let calculateMortgageSummary (principal: decimal) (annualRate: decimal) (years: int) (frequency: PaymentFrequency) (extraPayment: decimal) (startDate: DateTime option) : MortgageSummary =
        let schedule = generateMortgageSchedule principal annualRate years frequency extraPayment
        let regularPayment = calculateMortgagePayment principal annualRate years frequency
        
        let totalInterest = 
            schedule 
            |> List.sumBy (fun p -> p.InterestPortion)
        
        let totalPaid = 
            schedule 
            |> List.sumBy (fun p -> p.PaymentAmount)
        
        let payoffDate =
            match startDate with
            | Some start ->
                let months = 
                    match frequency with
                    | Monthly -> schedule.Length
                    | BiWeekly -> schedule.Length / 2
                    | Weekly -> schedule.Length / 4
                Some (start.AddMonths(months))
            | None -> None
        
        {
            LoanAmount = principal
            InterestRate = annualRate
            TermYears = years
            PaymentFrequency = frequency
            RegularPayment = regularPayment
            TotalPayments = schedule.Length
            TotalInterest = totalInterest
            TotalPaid = totalPaid
            PayoffDate = payoffDate
        }
    
    /// Calculate time and interest saved with extra payments
    let calculatePayoffAcceleration (principal: decimal) (annualRate: decimal) (years: int) (frequency: PaymentFrequency) (extraPayment: decimal) : (int * decimal * decimal) =
        let regularSchedule = generateMortgageSchedule principal annualRate years frequency 0m
        let acceleratedSchedule = generateMortgageSchedule principal annualRate years frequency extraPayment
        
        let paymentsSaved = regularSchedule.Length - acceleratedSchedule.Length
        let regularInterest = regularSchedule |> List.sumBy (fun p -> p.InterestPortion)
        let acceleratedInterest = acceleratedSchedule |> List.sumBy (fun p -> p.InterestPortion)
        let interestSaved = regularInterest - acceleratedInterest
        
        (paymentsSaved, interestSaved, regularInterest)


/// Car and auto loan calculations
module CarLoanCalculations =
    
    type CarLoanDetails = {
        VehiclePrice: decimal
        DownPayment: decimal
        TradeInValue: decimal
        LoanAmount: decimal
        InterestRate: decimal
        TermMonths: int
        MonthlyPayment: decimal
        TotalInterest: decimal
        TotalCost: decimal
        SalesTax: decimal
        Fees: decimal
    }
    
    /// Calculate car loan with all fees and taxes
    let calculateCarLoan (vehiclePrice: decimal) (downPayment: decimal) (tradeInValue: decimal) (salesTaxRate: decimal) (fees: decimal) (annualRate: decimal) (termMonths: int) : CarLoanDetails =
        let salesTax = vehiclePrice * (salesTaxRate / 100m)
        let totalPrice = vehiclePrice + salesTax + fees
        let loanAmount = totalPrice - downPayment - tradeInValue
        
        let monthlyRate = annualRate / 12m / 100m
        let monthlyPayment =
            if monthlyRate = 0m then
                loanAmount / decimal termMonths
            else
                let rate = float monthlyRate
                let n = float termMonths
                let p = float loanAmount
                let payment = p * (rate * (1.0 + rate) ** n) / ((1.0 + rate) ** n - 1.0)
                decimal payment
        
        let totalPaid = monthlyPayment * decimal termMonths
        let totalInterest = totalPaid - loanAmount
        let totalCost = downPayment + tradeInValue + totalPaid
        
        {
            VehiclePrice = vehiclePrice
            DownPayment = downPayment
            TradeInValue = tradeInValue
            LoanAmount = loanAmount
            InterestRate = annualRate
            TermMonths = termMonths
            MonthlyPayment = monthlyPayment
            TotalInterest = totalInterest
            TotalCost = totalCost
            SalesTax = salesTax
            Fees = fees
        }
    
    /// Compare lease vs buy decision
    let compareLeaseToBuy (vehiclePrice: decimal) (leaseMonthlyPayment: decimal) (leaseTermMonths: int) (buyMonthlyPayment: decimal) (buyTermMonths: int) (residualValue: decimal) : (decimal * decimal * decimal) =
        let totalLeaseCost = leaseMonthlyPayment * decimal leaseTermMonths
        let totalBuyCost = buyMonthlyPayment * decimal buyTermMonths
        let equityGained = vehiclePrice - totalBuyCost + residualValue
        
        (totalLeaseCost, totalBuyCost, equityGained)


/// Student loan calculations
module StudentLoanCalculations =
    
    type RepaymentPlan = Standard | GraduatedRepayment | ExtendedRepayment | IncomeBasedRepayment
    
    type StudentLoanSummary = {
        LoanBalance: decimal
        InterestRate: decimal
        Plan: RepaymentPlan
        MonthlyPayment: decimal
        TotalPayments: int
        TotalInterest: decimal
        TotalPaid: decimal
        InterestCapitalization: decimal
    }
    
    /// Calculate student loan payment under standard plan
    let calculateStudentLoanPayment (principal: decimal) (annualRate: decimal) (termYears: int) : decimal =
        let monthlyRate = annualRate / 12m / 100m
        let numPayments = termYears * 12
        
        if monthlyRate = 0m then
            principal / decimal numPayments
        else
            let rate = float monthlyRate
            let n = float numPayments
            let p = float principal
            let payment = p * (rate * (1.0 + rate) ** n) / ((1.0 + rate) ** n - 1.0)
            decimal payment
    
    /// Calculate income-based repayment (simplified)
    let calculateIncomeBasedPayment (annualIncome: decimal) (familySize: int) (discretionaryIncomePercent: decimal) : decimal =
        let povertyGuideline = 
            match familySize with
            | 1 -> 15060m
            | 2 -> 20440m
            | 3 -> 25820m
            | 4 -> 31200m
            | n -> 31200m + (decimal (n - 4) * 5380m)
        
        let discretionaryIncome = max 0m (annualIncome - (povertyGuideline * 1.5m))
        let monthlyPayment = (discretionaryIncome * discretionaryIncomePercent / 100m) / 12m
        monthlyPayment
    
    /// Calculate total interest with grace period and deferment
    let calculateStudentLoanWithDeferment (principal: decimal) (annualRate: decimal) (gracePeriodMonths: int) (defermentMonths: int) (termYears: int) : StudentLoanSummary =
        let monthlyRate = annualRate / 12m / 100m
        
        // Interest accrued during grace and deferment (capitalized)
        let gracePeriodInterest = principal * monthlyRate * decimal gracePeriodMonths
        let defermentInterest = (principal + gracePeriodInterest) * monthlyRate * decimal defermentMonths
        let capitalizedInterest = gracePeriodInterest + defermentInterest
        let newPrincipal = principal + capitalizedInterest
        
        let monthlyPayment = calculateStudentLoanPayment newPrincipal annualRate termYears
        let numPayments = termYears * 12
        let totalPaid = monthlyPayment * decimal numPayments
        let totalInterest = totalPaid - newPrincipal + capitalizedInterest
        
        {
            LoanBalance = newPrincipal
            InterestRate = annualRate
            Plan = Standard
            MonthlyPayment = monthlyPayment
            TotalPayments = numPayments
            TotalInterest = totalInterest
            TotalPaid = totalPaid
            InterestCapitalization = capitalizedInterest
        }
    
    /// Calculate loan forgiveness timeline (Public Service Loan Forgiveness)
    let calculateLoanForgiveness (principal: decimal) (annualRate: decimal) (monthlyPayment: decimal) (forgivenessMonths: int) : (decimal * decimal * decimal) =
        let monthlyRate = annualRate / 12m / 100m
        
        let rec accumulatePayments month balance totalPaid totalInterest =
            if month >= forgivenessMonths || balance <= 0m then
                (balance, totalPaid, totalInterest)
            else
                let interest = balance * monthlyRate
                let principal = monthlyPayment - interest
                let newBalance = max 0m (balance - principal)
                accumulatePayments (month + 1) newBalance (totalPaid + monthlyPayment) (totalInterest + interest)
        
        let (remainingBalance, totalPaid, totalInterest) = accumulatePayments 0 principal 0m 0m
        (remainingBalance, totalPaid, totalInterest)


/// Debt analysis and optimization
module DebtAnalysis =
    
    type DebtItem = {
        Name: string
        Balance: decimal
        InterestRate: decimal
        MinimumPayment: decimal
    }
    
    type DebtSummary = {
        TotalDebt: decimal
        WeightedAverageRate: decimal
        TotalMinimumPayment: decimal
        DebtToIncomeRatio: decimal option
        MonthsToPayoff: int
        TotalInterest: decimal
    }
    
    /// Calculate debt-to-income ratio
    let calculateDebtToIncomeRatio (monthlyDebtPayments: decimal) (monthlyGrossIncome: decimal) : decimal option =
        if monthlyGrossIncome <= 0m then None
        else Some ((monthlyDebtPayments / monthlyGrossIncome) * 100m)
    
    /// Calculate weighted average interest rate
    let calculateWeightedAverageRate (debts: DebtItem list) : decimal =
        let totalDebt = debts |> List.sumBy (fun d -> d.Balance)
        if totalDebt = 0m then 0m
        else
            debts
            |> List.sumBy (fun d -> d.Balance * d.InterestRate)
            |> fun total -> total / totalDebt
    
    /// Debt avalanche method (highest interest first)
    let debtAvalanche (debts: DebtItem list) (extraPayment: decimal) : (string * int * decimal) list =
        let sortedDebts = debts |> List.sortByDescending (fun d -> d.InterestRate)
        
        let rec payoffDebt remainingDebts monthsElapsed totalInterest results =
            match remainingDebts with
            | [] -> List.rev results
            | debt :: rest ->
                let monthlyRate = debt.InterestRate / 12m / 100m
                let payment = debt.MinimumPayment + extraPayment
                
                let rec payoffSingleDebt balance months interest =
                    if balance <= 0m then (months, interest)
                    else
                        let interestCharge = balance * monthlyRate
                        let principalPayment = min (payment - interestCharge) balance
                        let newBalance = balance - principalPayment
                        payoffSingleDebt newBalance (months + 1) (interest + interestCharge)
                
                let (monthsToPayoff, interestPaid) = payoffSingleDebt debt.Balance 0 0m
                let result = (debt.Name, monthsToPayoff, interestPaid)
                payoffDebt rest (monthsElapsed + monthsToPayoff) (totalInterest + interestPaid) (result :: results)
        
        payoffDebt sortedDebts 0 0m []
    
    /// Debt snowball method (smallest balance first)
    let debtSnowball (debts: DebtItem list) (extraPayment: decimal) : (string * int * decimal) list =
        let sortedDebts = debts |> List.sortBy (fun d -> d.Balance)
        debtAvalanche sortedDebts extraPayment // Same logic, different sort


/// Savings and investment calculations
module SavingsCalculations =
    
    type SavingsGoal = {
        TargetAmount: decimal
        CurrentSavings: decimal
        MonthlyContribution: decimal
        AnnualReturnRate: decimal
        MonthsToGoal: int
        TotalContributions: decimal
        TotalInterestEarned: decimal
    }
    
    type InvestmentProjection = {
        InitialInvestment: decimal
        MonthlyContribution: decimal
        AnnualReturnRate: decimal
        Years: int
        FutureValue: decimal
        TotalContributions: decimal
        TotalGains: decimal
    }
    
    /// Calculate savings goal timeline
    let calculateSavingsGoal (targetAmount: decimal) (currentSavings: decimal) (monthlyContribution: decimal) (annualRate: decimal) : SavingsGoal option =
        if monthlyContribution <= 0m then None
        else
            let monthlyRate = annualRate / 12m / 100m
            
            let rec calculateMonths balance months totalContrib totalInterest =
                if balance >= targetAmount then
                    Some {
                        TargetAmount = targetAmount
                        CurrentSavings = currentSavings
                        MonthlyContribution = monthlyContribution
                        AnnualReturnRate = annualRate
                        MonthsToGoal = months
                        TotalContributions = totalContrib
                        TotalInterestEarned = totalInterest
                    }
                elif months > 1200 then None // Cap at 100 years
                else
                    let interest = balance * monthlyRate
                    let newBalance = balance + monthlyContribution + interest
                    calculateMonths newBalance (months + 1) (totalContrib + monthlyContribution) (totalInterest + interest)
            
            calculateMonths currentSavings 0 0m 0m
    
    /// Calculate future value of investment with regular contributions
    let calculateInvestmentProjection (initialInvestment: decimal) (monthlyContribution: decimal) (annualRate: decimal) (years: int) : InvestmentProjection =
        let monthlyRate = annualRate / 12m / 100m
        let numMonths = years * 12
        
        let rec accumulate month balance totalContrib =
            if month >= numMonths then (balance, totalContrib)
            else
                let interest = balance * monthlyRate
                let newBalance = balance + monthlyContribution + interest
                accumulate (month + 1) newBalance (totalContrib + monthlyContribution)
        
        let (futureValue, totalContributions) = accumulate 0 initialInvestment 0m
        let totalGains = futureValue - initialInvestment - totalContributions
        
        {
            InitialInvestment = initialInvestment
            MonthlyContribution = monthlyContribution
            AnnualReturnRate = annualRate
            Years = years
            FutureValue = futureValue
            TotalContributions = totalContributions
            TotalGains = totalGains
        }
    
    /// Calculate compound interest with different compounding frequencies
    let calculateCompoundInterestAdvanced (principal: decimal) (annualRate: decimal) (years: int) (compoundingsPerYear: int) : decimal =
        let rate = float annualRate / 100.0
        let n = float compoundingsPerYear
        let t = float years
        let p = float principal
        let futureValue = p * (1.0 + rate / n) ** (n * t)
        decimal futureValue
    
    /// Calculate required monthly savings to reach goal
    let calculateRequiredMonthlySavings (targetAmount: decimal) (currentSavings: decimal) (annualRate: decimal) (years: int) : decimal option =
        let monthlyRate = annualRate / 12m / 100m
        let numMonths = years * 12
        
        if numMonths <= 0 then None
        else
            // Future value of current savings
            let fvCurrentSavings = 
                if monthlyRate = 0m then currentSavings
                else 
                    let rate = float monthlyRate
                    let n = float numMonths
                    let cs = float currentSavings
                    decimal (cs * (1.0 + rate) ** n)
            
            let remainingNeeded = targetAmount - fvCurrentSavings
            
            if remainingNeeded <= 0m then Some 0m
            elif monthlyRate = 0m then Some (remainingNeeded / decimal numMonths)
            else
                // PMT formula: PMT = FV * r / ((1 + r)^n - 1)
                let rate = float monthlyRate
                let n = float numMonths
                let fv = float remainingNeeded
                let pmt = fv * rate / ((1.0 + rate) ** n - 1.0)
                Some (decimal pmt)


/// Refinance and comparison calculations
module RefinanceCalculations =
    
    type RefinanceComparison = {
        CurrentLoan: MortgageCalculations.MortgageSummary
        NewLoan: MortgageCalculations.MortgageSummary
        ClosingCosts: decimal
        BreakEvenMonths: int
        MonthlyPaymentSavings: decimal
        TotalInterestSavings: decimal
        NetSavings: decimal
        IsWorthwhile: bool
    }
    
    /// Compare current loan to refinance option
    let compareRefinance 
        (currentBalance: decimal) 
        (currentRate: decimal) 
        (currentRemainingMonths: int)
        (newRate: decimal)
        (newTermYears: int)
        (closingCosts: decimal) : RefinanceComparison =
        
        let currentYears = currentRemainingMonths / 12
        let currentSummary = MortgageCalculations.calculateMortgageSummary currentBalance currentRate currentYears MortgageCalculations.Monthly 0m None
        let newSummary = MortgageCalculations.calculateMortgageSummary currentBalance newRate newTermYears MortgageCalculations.Monthly 0m None
        
        let monthlyPaymentSavings = currentSummary.RegularPayment - newSummary.RegularPayment
        let totalInterestSavings = currentSummary.TotalInterest - newSummary.TotalInterest
        let netSavings = totalInterestSavings - closingCosts
        
        let breakEvenMonths =
            if monthlyPaymentSavings <= 0m then 9999
            else int (Math.Ceiling(float (closingCosts / monthlyPaymentSavings)))
        
        let isWorthwhile = netSavings > 0m && breakEvenMonths < (newTermYears * 12)
        
        {
            CurrentLoan = currentSummary
            NewLoan = newSummary
            ClosingCosts = closingCosts
            BreakEvenMonths = breakEvenMonths
            MonthlyPaymentSavings = monthlyPaymentSavings
            TotalInterestSavings = totalInterestSavings
            NetSavings = netSavings
            IsWorthwhile = isWorthwhile
        }
    
    /// Calculate effective interest rate including fees
    let calculateEffectiveRate (loanAmount: decimal) (nominalRate: decimal) (fees: decimal) (termYears: int) : decimal =
        let totalCost = loanAmount + fees
        let effectiveAmount = loanAmount - fees
        
        if effectiveAmount <= 0m then nominalRate
        else
            // Approximate effective rate
            let feePercentage = (fees / loanAmount) * 100m
            let annualizedFee = feePercentage / decimal termYears
            nominalRate + annualizedFee


/// Budget category analysis
module BudgetCategoryAnalyzer =
    
    type CategorySpending = {
        CategoryName: string
        BudgetedAmount: decimal
        ActualSpent: decimal
        Variance: decimal
        VariancePercent: decimal
        PercentOfTotal: decimal
    }
    
    type BudgetAnalysis = {
        TotalBudgeted: decimal
        TotalSpent: decimal
        TotalVariance: decimal
        OverspentCategories: CategorySpending list
        UnderspentCategories: CategorySpending list
        OnTrackCategories: CategorySpending list
        Recommendations: string list
    }
    
    /// Analyze spending patterns by category
    let analyzeCategorySpending (categories: (string * decimal * decimal) list) : BudgetAnalysis =
        let totalBudgeted = categories |> List.sumBy (fun (_, budgeted, _) -> budgeted)
        let totalSpent = categories |> List.sumBy (fun (_, _, spent) -> spent)
        let totalVariance = totalSpent - totalBudgeted
        
        let categoryDetails =
            categories
            |> List.map (fun (name, budgeted, spent) ->
                let variance = spent - budgeted
                let variancePercent = if budgeted = 0m then 0m else (variance / budgeted * 100m)
                let percentOfTotal = if totalSpent = 0m then 0m else (spent / totalSpent * 100m)
                {
                    CategoryName = name
                    BudgetedAmount = budgeted
                    ActualSpent = spent
                    Variance = variance
                    VariancePercent = variancePercent
                    PercentOfTotal = percentOfTotal
                })
        
        let overspent = categoryDetails |> List.filter (fun c -> c.Variance > c.BudgetedAmount * 0.05m)
        let underspent = categoryDetails |> List.filter (fun c -> c.Variance < -c.BudgetedAmount * 0.05m)
        let onTrack = categoryDetails |> List.filter (fun c -> abs c.Variance <= c.BudgetedAmount * 0.05m)
        
        let recommendations =
            [
                if totalVariance > 0m then "Overall spending exceeds budget. Consider reducing discretionary expenses."
                if overspent.Length > 0 then sprintf "Focus on reducing spending in: %s" (overspent |> List.map (fun c -> c.CategoryName) |> String.concat ", ")
                if underspent.Length > 0 then sprintf "Consider reallocating funds from: %s" (underspent |> List.map (fun c -> c.CategoryName) |> String.concat ", ")
                if totalVariance < 0m && abs totalVariance > totalBudgeted * 0.1m then "Great job staying under budget! Consider increasing savings."
            ]
        
        {
            TotalBudgeted = totalBudgeted
            TotalSpent = totalSpent
            TotalVariance = totalVariance
            OverspentCategories = overspent
            UnderspentCategories = underspent
            OnTrackCategories = onTrack
            Recommendations = recommendations
        }
    
    /// Calculate recommended budget adjustments
    let recommendBudgetAdjustments (categories: (string * decimal * decimal) list) (targetTotalBudget: decimal) : (string * decimal * decimal) list =
        let currentTotal = categories |> List.sumBy (fun (_, budgeted, _) -> budgeted)
        let adjustmentRatio = targetTotalBudget / currentTotal
        
        categories
        |> List.map (fun (name, budgeted, spent) ->
            let adjustedBudget = budgeted * adjustmentRatio
            let difference = adjustedBudget - budgeted
            (name, adjustedBudget, difference))
