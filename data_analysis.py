import glob

import xlsxwriter
import statistics
from datetime import datetime

# TODO: Change Log Directory
LOG_DIRECTORY = "Assets/logs/"


def parse_log_directory(log_dir):
    log_files = glob.glob(log_dir + "/*")

    retval = []

    for f_path in log_files:
        if ".meta" in f_path:
            continue
        f_name = f_path.split("\\")[-1]

        with open(f_path, "r", encoding="utf-8", errors="ignore") as f:
            lines = f.readlines()

        retval.append([f_name, lines])

    return retval


def get_trials(lines):
    trials = []
    indexes = []
    for i in range(len(lines)):
        line = lines[i]
        if "Initialization of Logging File Completed" in line:
            continue
        if "Menu Control Started" in line:
            indexes.append(i)

    starting_index = indexes[0]
    for i in indexes[1:]:
        trials.append(lines[starting_index:i])
        starting_index = i

    trials.append(lines[starting_index:])

    return trials


def get_overshoots(steps, goal):
    overshoot_steps = []
    overshoots = []
    start_i = 0
    for i in range(len(steps)):
        if int(steps[start_i]) < goal and int(steps[i]) < goal:
            continue
        if int(steps[start_i]) > goal and int(steps[i]) > goal:
            continue

        overshoot_steps.append(steps[start_i:i])
        start_i = i

    for overshoot_list in overshoot_steps[1:]:
        if int(overshoot_list[0]) == goal:
            continue
        cur_biggest = goal

        for overshoot in overshoot_list:
            if abs(goal - int(overshoot)) > abs(goal - cur_biggest):
                cur_biggest = int(overshoot)

        temp = goal - cur_biggest

        overshoots.append(str(temp * -1))

    total_os = 0
    for x in overshoots:
        total_os += abs(int(x))

    return overshoots, total_os


def get_round_data(round_lines, num_btn, trial_num, control_scheme, cur_round):
    assert "Button Clicked" in round_lines[-1]
    goal_btn = int(round_lines[0].split("[DEBUG]")[1].strip().split(",,")[1].split(":")[1].strip())
    selected_btns = []
    miss_clicks = 0

    # Get Time Of Round
    starting_time_str = round_lines[0].split("]")[0].replace("[", "")
    starting_time = datetime.strptime(starting_time_str, "%H:%M:%S.%f")
    end_time_str = round_lines[-1].split("]")[0].replace("[", "")
    end_time = datetime.strptime(end_time_str, "%H:%M:%S.%f")
    delta = end_time - starting_time

    for trial_line in round_lines:

        if "Button Clicked:" in trial_line:
            if trial_line != round_lines[-1]:
                miss_clicks += 1
            continue

        trial_line_info = trial_line.split("[DEBUG]")[1].strip().split(",,")
        active_btns = trial_line_info[2].split(":")[1].strip()

        if control_scheme == "3" and not active_btns:
            continue

        active_btns = active_btns.split(",")

        if control_scheme == "1":
            selected_btn = trial_line_info[0].split(":")[1].strip()
        elif control_scheme == "3":
            # We are treating "best case" in active buttons as selected button
            selected_btn = 1000
            for btn in active_btns:
                btn = btn.strip()
                if not btn:
                    continue
                btn = int(btn)

                # We are treating the shortest-pathed active button as the "selected button"
                if abs(btn - goal_btn) < abs(selected_btn - goal_btn):
                    selected_btn = btn
        else:
            selected_btn = -1

        if str(selected_btn) not in selected_btns or str(selected_btn) != selected_btns[-1]:
            selected_btns.append(str(selected_btn))

    overshoots, total_os = get_overshoots(selected_btns, goal_btn)

    return [
        f"{trial_num}",  # TrialNum
        control_scheme,  # Control Scheme
        num_btn,  # Num Buttons
        f"{cur_round}",  # Round
        f"{selected_btns[0]}",  # Starting Number
        f"{goal_btn}",  # Target Number
        str(delta),  # Target Acquisition
        f"{miss_clicks}",  # Miss clicks
        ",".join(overshoots),  # Overshoots
        f"{total_os}",  # Total Overshot
        f"{abs(int(goal_btn) - int(selected_btns[0]))}",  # Optimal Path Length
        f"{len(selected_btns) - 1}",  # Acquired Path Length
        ",".join(selected_btns),  # Acquired Path
    ]


def get_trial_data(trial_lines, num_btn, trial_num, control_scheme):
    goal_btn = int(trial_lines[0].split("[DEBUG]")[1].strip().split(",,")[1].split(":")[1].strip())
    rounds = []
    goals = []
    trial_lines.append("")
    starting_indx = 0
    for i in range(len(trial_lines)):
        if not trial_lines[i]:
            continue
        if "Button Clicked:" not in trial_lines[i]:
            continue
        if not int(trial_lines[i].split("[DEBUG]")[1].strip().split(":")[1].strip()) == goal_btn:
            continue
        goals.append(goal_btn)
        rounds.append(trial_lines[starting_indx:i + 1])
        starting_indx = i + 1
        if not trial_lines[i + 1]:
            continue
        new_goal_btn = int(trial_lines[i + 1].split("[DEBUG]")[1].strip().split(",,")[1].split(":")[1].strip())
        goal_btn = new_goal_btn

    assert len(rounds) == 10

    retval = []

    for i in range(len(rounds)):
        retval.append(get_round_data(rounds[i], num_btn, trial_num, control_scheme, i + 1))

    return retval


def main():
    logs = parse_log_directory(LOG_DIRECTORY)

    workbook = xlsxwriter.Workbook(f'./results.xlsx')

    worksheets = {}
    
    summary_data = {}

    for log_file in logs:

        worksheets[log_file[0]] = workbook.add_worksheet(log_file[0])

        columns = [
            {'header': 'TrialNum'},
            {'header': 'Control Scheme'},
            {'header': 'Num Buttons'},
            {'header': 'Round'},
            {'header': 'Starting Number'},
            {'header': 'Target Number'},
            {'header': 'Target Acquisition'},
            {'header': 'Miss clicks'},
            {'header': 'Overshoots'},
            {'header': 'Total Overshot'},
            {'header': 'Optimal Path Length'},
            {'header': 'Acquired Path Length'},
            {'header': 'Acquired Path'},
        ]
        trials = get_trials(log_file[1])
        log_data = []

        summary_data[log_file[0]] = {}

        for i in range(len(trials)):
            trial_lines = trials[i]
            trial_info = trial_lines[0].split("[DEBUG]")[1].strip().split(",")
            control_scheme = trial_info[1].replace("Control Scheme: ", "").strip()
            num_btn = trial_info[2].replace("Num Buttons: ", "")
            trial_lines = trial_lines[1:]
            if "1" in control_scheme:
                trial_data = get_trial_data(trial_lines, num_btn, i, "1")
            else:
                trial_data = get_trial_data(trial_lines, num_btn, i, "3")
            log_data += trial_data

            summary_data[log_file[0]][i+1] = trial_data

        worksheets[log_file[0]].set_column(f'A:{chr(len(columns) + 64)}', 20)
        worksheets[log_file[0]].add_table(
            f'A1:{chr(len(columns) + 64)}{len(log_data) + 1}',
            {
                'data': log_data,
                'columns': columns
            }
        )

    worksheets["Summary"] = workbook.add_worksheet("Summary")
    columns = [
        {'header': 'FileName'},
        {'header': 'TrialNum'},
        {'header': 'Control Scheme'},
        {'header': 'Num Buttons'},
        {'header': 'Target Acquisition Mean (Seconds)'},
        {'header': 'Target Acquisition StdDev (Seconds)'},
        {'header': 'Miss clicks Mean'},
        {'header': 'Miss clicks StdDev'},
        {'header': 'Total Overshot Mean'},
        {'header': 'Total Overshot StdDev'},
        {'header': 'Optimal Path/Acquired Path Mean'},
        {'header': 'Optimal Path/Acquired Path StdDev'},
    ]
    formatted_summary_data = []

    for file_name in summary_data.keys():
        file_data = summary_data[file_name]
        for trial_num in file_data.keys():
            trial_data = file_data[trial_num]
            control_scheme = trial_data[0][1]
            num_buttons = trial_data[0][2]
            target_acq_results = []
            miss_clicks_results = []
            overshoot_results = []
            pathing_results = []
            for round_data in trial_data:
                target_acq_parts = round_data[6].split(":")
                target_acq = int(target_acq_parts[0]) * 3600 + int(target_acq_parts[1]) * 60 + float(target_acq_parts[2])
                target_acq_results.append(target_acq)
                miss_clicks_results.append(int(round_data[7]))
                overshoot_results.append(int(round_data[9]))
                if int(round_data[11]) == 0:
                    pathing_results.append(1)
                else:
                    pathing_results.append(int(round_data[10]) / int(round_data[11]))

            formatted_summary_data.append([
                file_name,  # 'FileName'
                trial_num,  # 'TrialNum'
                control_scheme,  # 'Control Scheme'
                num_buttons,  # 'Num Buttons'
                statistics.mean(target_acq_results),  # 'Target Acquisition Mean'
                statistics.stdev(target_acq_results),  # 'Target Acquisition StdDev'
                statistics.mean(miss_clicks_results),  # 'Miss clicks Mean'
                statistics.stdev(miss_clicks_results),  # 'Miss clicks StdDev'
                statistics.mean(overshoot_results),  # 'Total Overshot Mean'
                statistics.stdev(overshoot_results),  # 'Total Overshot StdDev'
                statistics.mean(pathing_results),  # Optimal Path/Acquired Path Mean'
                statistics.stdev(pathing_results),  # 'Optimal Path/Acquired Path StdDev'
            ])

    worksheets["Summary"].set_column(f'A:{chr(len(columns) + 64)}', 20)
    worksheets["Summary"].add_table(
        f'A1:{chr(len(columns) + 64)}{len(formatted_summary_data) + 1}',
        {
            'data': formatted_summary_data,
            'columns': columns
        }
    )

    workbook.close()


if __name__ == "__main__":
    main()
