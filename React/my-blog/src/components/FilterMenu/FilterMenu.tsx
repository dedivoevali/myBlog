import {
    Box,
    Button,
    Chip,
    FormControlLabel,
    Input,
    InputLabel,
    MenuItem,
    Paper,
    Select,
    Switch,
    Typography
} from '@mui/material';
import React, {useState} from 'react';
import {Filter, FilterLogicalOperator} from '../../shared/api/types/paging';
import {FilterMenuProps} from './FilterMenuProps';
import {useSearchParams} from "react-router-dom";
import {requestFiltersToBrowserQueryString} from "../../shared/assets/requestFiltersToBrowserQueryString";
import SearchIcon from '@mui/icons-material/Search';
import styles from "./filter-menu.module.scss";

const FilterMenu = ({availableFilters, width, requestFilters, setFilters}: FilterMenuProps) => {

    const [searchParams, setSearchParams] = useSearchParams();

    const [doCombineFiltersWithOrOperator, setDoCombineFiltersWithOrOperator] = useState<boolean>(requestFilters.logicalOperator === FilterLogicalOperator.Or);
    const [filterPathDropdown, setFilterPathDropdown] = useState<string>("");
    const [filterValue, setFilterValue] = useState<string>("");


    const alreadyHasNoSuchFilter = (filter: Filter) => {
        return !requestFilters.filters.some((val) => val.path === filter.path && val.value === filter.value);
    }

    const handleDeleteFilter = (triggeredFilter: Filter) => {


        let removedTriggeredFilter = requestFilters.filters.filter((filter) => (filter.path !== triggeredFilter.path) || (filter.value !== triggeredFilter.value));

        setSearchParams(requestFiltersToBrowserQueryString({
            filters: removedTriggeredFilter,
            logicalOperator: requestFilters.logicalOperator
        }));

        setFilters({
            logicalOperator: requestFilters.logicalOperator,
            filters: removedTriggeredFilter
        });
    }

    const handleAddFilter = () => {
        const newFilters = requestFilters.filters.concat({path: filterPathDropdown, value: filterValue});


        setSearchParams(requestFiltersToBrowserQueryString({
            filters: newFilters,
            logicalOperator: requestFilters.logicalOperator
        }));

        if (filterPathDropdown && filterValue && alreadyHasNoSuchFilter({
            path: filterPathDropdown,
            value: filterValue
        })) {
            setFilters({
                logicalOperator: requestFilters.logicalOperator,
                filters: newFilters
            });
        }

    }

    const handleChangeOfLogicalOperator = () => {

        const newLogicalOperator = doCombineFiltersWithOrOperator ? FilterLogicalOperator.And : FilterLogicalOperator.Or;

        setSearchParams(requestFiltersToBrowserQueryString({
            filters: requestFilters.filters,
            logicalOperator: newLogicalOperator
        }));

        if (doCombineFiltersWithOrOperator) {
            setFilters({...requestFilters, logicalOperator: FilterLogicalOperator.And});
        } else {
            setFilters({...requestFilters, logicalOperator: FilterLogicalOperator.Or});
        }

        setDoCombineFiltersWithOrOperator(!doCombineFiltersWithOrOperator);
    }


    return (
        <Paper elevation={12} style={{ width }} className={styles["menu-wrapper"]}>

            <Box className={styles["header"]}>
                <Typography className={styles["header__caption"]}>
                    <SearchIcon fontSize={"large"}/>
                    Search
                </Typography>
            </Box>

            <Box className={styles["buttons"]}>

                <Box className={styles["buttons__dropdown"]}>
                    <InputLabel id="filter-selector">Filter name</InputLabel>
                    <Select className={styles["buttons__dropdown__selector"]} value={filterPathDropdown}
                            onChange={(e) => setFilterPathDropdown(e.target.value)}>
                        {availableFilters.map((value, index) => <MenuItem value={value} key={index}>{value}</MenuItem>)}
                    </Select>
                </Box>

                <Box className={styles["buttons__search"]}>
                    <InputLabel id="filter-value-input">Filter value</InputLabel>
                    <Input
                        fullWidth
                        placeholder="Search..."
                        value={filterValue}
                        onChange={e => setFilterValue(e.target.value)}
                        type="text"
                        name="filter-value"/>
                </Box>

                <Box>
                    <Button disabled={filterPathDropdown.length === 0 || filterValue.length === 0} variant="outlined"
                            onClick={handleAddFilter}>Add filter</Button>
                </Box>
            </Box>

            {requestFilters.filters.length > 1 &&
                <Box className={styles["search-separately"]}>
                    <InputLabel id="filter-intersection">Search separately</InputLabel>
                    <FormControlLabel
                        className={styles["search-separately__input"]}
                        control={<Switch checked={doCombineFiltersWithOrOperator}
                            onChange={handleChangeOfLogicalOperator} />}
                        label="" />
                </Box>
            }
            <>
                {requestFilters?.filters.map((filter, index) =>
                    <Chip color="secondary" key={index} onDelete={() => handleDeleteFilter(filter)} label={
                        <>
                            <span style={{fontWeight: "bold"}}>{filter.path}</span> : {filter.value}
                        </>
                    }/>)}
            </>
        </Paper>
    );
};

export {FilterMenu};