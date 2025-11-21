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
