use super::super::puzzle::Puzzle;
use std::fs::File;
use std::io::{self, BufRead};
use std::path::Path;

pub struct Solution {}

impl Puzzle for Solution {
    fn part1(&self) {
        // Read file:
        if let Ok(lines) = read_lines("input/year2021/day1.txt") {
            // Make into integer iterator:
            let mut int_iter = lines.into_iter().map(|line| {
                line.expect("WHAT")
                    .parse::<i32>()
                    .expect("Unable to parse into integer: {line}")
            });
            let first = match int_iter.nth(0) {
                Some(k) => k,
                None => panic!("No first element found!"),
            };
            let answer = int_iter
                .scan(first, |state, x| {
                    let temp = x - *state;
                    *state = x;
                    Some(temp)
                })
                .filter(|&x| x > 0)
                .count();

            println!("Part 1: {answer}");
        } else {
            println!("Something went wrong in reading file...");
        }
    }
    fn part2(&self) {
        if let Ok(lines) = read_lines("input/year2021/day1.txt") {
            let mut int_iter = lines.into_iter().map(|line| {
                line.expect("...")
                    .parse::<i32>()
                    .expect("Unable to parse int {line}")
            });

            // Get first two elements:
            let first = match int_iter.nth(0) {
                Some(k) => k,
                None => panic!("No first element"),
            };
            let second = match int_iter.nth(0) {
                Some(k) => k,
                None => panic!("No second element"),
            };

            let mut rolling_window = int_iter.scan((first, second), |state, x| {
                let (prev, prev2) = *state;
                *state = (x, prev);
                Some(prev + prev2 + x)
            });

            let first = match rolling_window.nth(0) {
                Some(k) => k,
                None => panic!("No first element"),
            };

            let answer = rolling_window
                .scan(first, |state, x| {
                    let temp = x - *state;
                    *state = x;
                    Some(temp)
                })
                .filter(|&x| x > 0)
                .count();
            println!("Part 2: {}", answer);
        }
    }
}

fn read_lines<P>(filename: P) -> io::Result<io::Lines<io::BufReader<File>>>
where
    P: AsRef<Path>,
{
    let file = File::open(filename)?;
    Ok(io::BufReader::new(file).lines())
}
