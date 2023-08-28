import {NextPage} from "next";
import {useTranslation} from "react-i18next";
import ContentContainer from "@/components/design/ContentContainer";
import Section from "@/components/design/Section";
import useSWR from "swr";
import React, {useMemo, useState} from "react";
import {AccountingReportingPeriod, IncomeDetail} from "@/libs/models";
import {formatAmount, formatIsoMonth} from "@/libs/formats";
import {parseISO} from "date-fns";
import {ArrowSmallLeftIcon, ArrowSmallRightIcon} from "@heroicons/react/24/solid";

import dynamic from 'next/dynamic';

const ReactApexChart = dynamic(() => import('react-apexcharts'), {ssr: false})

const Reporting: NextPage = () => {
    const {t} = useTranslation("accounting");

    const periods = useSWR<AccountingReportingPeriod[]>("/data/accounting/reporting/periods");
    const [year, setYear] = useState<number>(new Date().getFullYear());

    const currentPeriod = useMemo(() => {
        let datas = periods.data ?? [];

        datas = datas.filter((period) => parseISO(period.startDate).getFullYear() === year);
        datas = datas.sort((a, b) => parseISO(a.startDate).getTime() - parseISO(b.startDate).getTime());

        for (let period of datas) {
            period.details = period.details.sort((a, b) => a.type?.localeCompare(b?.type ?? "") || a.unitPrice - b.unitPrice);
        }

        return datas;
    }, [periods.data, year]);

    const graphData = useMemo(() => {

        return {
            chart: {
                id: "income-reporting",
            },
            xaxis: {
                categories: currentPeriod.map((period) => formatIsoMonth(period.startDate)),
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '55%',
                    endingShape: 'rounded'
                },
            },
            stroke: {
                show: true,
                width: 2,
                colors: ['transparent']
            },
            legend: {
                show: true,
            },
            series: [
                {
                    name: t("reporting.income"),
                    data: currentPeriod.map((period) => period.income),
                    color: "#00b74a",
                },
                {
                    name: t("reporting.expenses"),
                    data: currentPeriod.map((period) => period.expense),
                    color: "#ff5252",
                },
            ]
        } as ApexCharts.ApexOptions;

    }, [currentPeriod]);

    function getType(detail: IncomeDetail) {
        
        if (detail.category?.length) {
            return detail.category;
        }
        
        switch (detail.type) {
            case "appointment":
                return t("reporting.details.appointments");
            case "document-expense":
                return t("reporting.details.document-expense");
            case "document-income":
                return t("reporting.details.document-income");
                
        }

        return detail.type;
    }

    return <ContentContainer>
        <Section>
            <h1>{t("reporting.title")}</h1>
            <>
                <div className={"grid grid-cols-8 cursor-pointer"}>
                    <div className={"col-start-1 w-8 m-auto text-primary"} onClick={() => setYear(year - 1)}>
                        <ArrowSmallLeftIcon/>
                    </div>

                    <h1 className={"text-center col-span-6"}>
                        {year}
                    </h1>

                    <div className={"col-start-8 1 w-8 m-auto text-primary"} onClick={() => setYear(year + 1)}>
                        <ArrowSmallRightIcon/>
                    </div>
                </div>
                <div className={"print:hidden"}>
                    <ReactApexChart options={graphData} series={graphData.series} type="bar" width={"100%"}
                                    height={500}/>
                </div>
            </>
        </Section>
        {graphData &&
            <Section>
                <h2>{t("reporting.details.title")}</h2>
                <>
                    {currentPeriod.map((period) => <div key={period.id} className={"pb-3"}>
                        <h3>{t("reporting.period")} : {formatIsoMonth(period.startDate)}</h3>
                        <div className={"grid grid-cols-4"}>
                            <div className={"underline"}>{t("reporting.details.type")}</div>
                            <div className={"underline text-right"}>{t("reporting.details.unitPrice")}</div>
                            <div className={"underline text-right"}>{t("reporting.details.count")}</div>
                            <div className={"underline text-right"}>{t("reporting.details.total")}</div>
                            <>
                                {period.details.map((detail) => <React.Fragment key={detail.id}>
                                    <div>
                                        {getType(detail)}
                                    </div>
                                    <div className={"text-right"}>
                                        {formatAmount(detail.unitPrice)} &euro;
                                    </div>
                                    <div className={"text-right"}>
                                        {detail.count}
                                    </div>
                                    <div className={"text-right"}>
                                        {formatAmount(detail.subTotal)} &euro;
                                    </div>
                                </React.Fragment>)}
                            </>
                        </div>
                        <div className={"grid grid-cols-4"}>
                            <div />
                            <div />
                            <div />
                            <div className={"text-right font-bold border-t"}>
                                {formatAmount(period.income - period.expense)} &euro;
                            </div>
                        </div>
                    </div>)}
                </>
            </Section>
        }
    </ContentContainer>;
}

export default Reporting;