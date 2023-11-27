use std::ops;

#[derive(Debug, Clone, Copy)]
pub struct Node {
    pub x: i32,
    pub y: i32,
}

impl ops::Add<Node> for Node {
    type Output = Node;

    fn add(self, _rhs: Node) -> Node {
        Node {
            x: self.x + _rhs.x,
            y: self.y + _rhs.y,
        }
    }
}

impl ops::Add<(i32, i32)> for Node {
    type Output = Node;

    fn add(self, _rhs: (i32, i32)) -> Node {
        Node {
            x: self.x + _rhs.0,
            y: self.y + _rhs.1,
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn add_node_works() {
        let node1 = Node { x: 1, y: 2 };
        let node2 = Node { x: 3, y: 4 };
        let sum = node1 + node2;
        assert_eq!(4, sum.x);
        assert_eq!(6, sum.y);
    }

    #[test]
    fn add_tangent_works() {
        let node1 = Node { x: 1, y: 2 };
        let sum = node1 + (3, 4);
        assert_eq!(4, sum.x);
        assert_eq!(6, sum.y);
    }
}
