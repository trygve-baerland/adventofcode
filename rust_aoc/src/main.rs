use std::env;

use puzzle::Puzzle;

mod puzzle;
mod year2021;

fn main() {
    let args: Vec<String> = env::args().collect();

    // Get year
    let year: i32 = args[1]
        .trim()
        .parse()
        .expect("Unable to parse year {args[1]}");

    let day: i32 = args[2]
        .trim()
        .parse()
        .expect("Unable to parse day {args[2]}");

    match year {
        2021 => match day {
            1 => {
                let sol = year2021::day1::Solution {};
                sol.part1();
                sol.part2();
            }
            other => {
                panic!("No solution for year 2021, day {other}");
            }
        },
        other => {
            panic!("No solutions for year {other}");
        }
    };
}
