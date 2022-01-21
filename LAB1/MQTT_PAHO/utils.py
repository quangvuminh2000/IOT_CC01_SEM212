import numpy as np
import math


def generate_random_location(x0, y0, r):
    """
    Generate the random location within the radius r of the position (x0, y0)

    Parameters
    ----------
    x0 : float
        Longitude of the starting location
    y0 : float
        Latitude of the starting location
    r : float
        The radius of the surrounding location (meters)

    Returns
    -------
    Tuple[float, float]
        New random position in the radius
    """

    # Convert radius to degree
    r_d = r/111000

    # Generate 2 iid values
    u, v = np.random.uniform(0, 1, 2)

    # Create a random manhattan distance from the current position
    w = r_d * math.sqrt(u)

    # Create a random orientation of the new position
    t = 2*math.pi*v

    # Generate new x,y such that x,y,w is 3 sides of a right triangle
    x = w*math.cos(t)
    y = w*math.sin(t)

    # Adjust the x-coordinate for the shrinking of the east-west distances
    new_x = x/math.cos(math.radians(y0))

    # New position
    new_longitude = new_x + x0
    new_latitude = y + y0

    return (new_longitude, new_latitude)


if __name__ == '__main__':
    posA = (106.6297, 10.8231)
    posB = generate_random_location(106.6297, 10.8231, 200)
    print(posB)
